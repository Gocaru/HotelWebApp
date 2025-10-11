using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Models.Api;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Manages invoices and payments for guest reservations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "ApiScheme")]
    public class InvoicesController : ControllerBase
    {
        private readonly HotelWebAppContext _context;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoicesController(
            HotelWebAppContext context,
            IPaymentService paymentService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves all invoices for the authenticated guest
        /// </summary>
        /// <returns>List of invoices with payment details and balance information</returns>
        // GET: api/invoices
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<InvoiceDto>>>> GetMyInvoices()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<List<InvoiceDto>>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var invoices = await _context.Invoices
                    .Include(i => i.Reservation)
                        .ThenInclude(r => r.Room)
                    .Include(i => i.Payments)
                    .Where(i => i.GuestId == userId)
                    .OrderByDescending(i => i.InvoiceDate)
                    .ToListAsync();

                var invoiceDtos = invoices.Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    ReservationId = i.ReservationId,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status.ToString(),
                    AmountPaid = i.Payments.Sum(p => p.Amount),
                    BalanceDue = i.TotalAmount - i.Payments.Sum(p => p.Amount),
                    RoomNumber = i.Reservation?.Room?.RoomNumber ?? "N/A",
                    CheckInDate = i.Reservation?.CheckInDate,
                    CheckOutDate = i.Reservation?.CheckOutDate,
                    Payments = i.Payments.Select(p => new PaymentDto
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentMethod.ToString(),
                        TransactionId = p.TransactionId
                    }).ToList()
                }).ToList();

                return Ok(new ApiResponse<List<InvoiceDto>>
                {
                    Success = true,
                    Data = invoiceDtos,
                    Message = "Invoices retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<InvoiceDto>>
                {
                    Success = false,
                    Message = "Error retrieving invoices",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific invoice
        /// </summary>
        /// <param name="id">The invoice ID</param>
        /// <returns>Complete invoice details including room charges, amenities, and payment history</returns>
        // GET: api/invoices/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetInvoice(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<InvoiceDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var invoice = await _context.Invoices
                    .Include(i => i.Reservation)
                        .ThenInclude(r => r.Room)
                    .Include(i => i.Reservation)
                        .ThenInclude(r => r.ReservationAmenities)
                            .ThenInclude(ra => ra.Amenity)
                    .Include(i => i.Payments)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    return NotFound(new ApiResponse<InvoiceDto>
                    {
                        Success = false,
                        Message = "Invoice not found"
                    });
                }

                if (invoice.GuestId != userId)
                {
                    return Forbid();
                }

                var invoiceDto = new InvoiceDto
                {
                    Id = invoice.Id,
                    ReservationId = invoice.ReservationId,
                    InvoiceDate = invoice.InvoiceDate,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status.ToString(),
                    AmountPaid = invoice.Payments.Sum(p => p.Amount),
                    BalanceDue = invoice.TotalAmount - invoice.Payments.Sum(p => p.Amount),
                    RoomNumber = invoice.Reservation?.Room?.RoomNumber ?? "N/A",
                    CheckInDate = invoice.Reservation?.CheckInDate,
                    CheckOutDate = invoice.Reservation?.CheckOutDate,
                    ReservationAmenities = invoice.Reservation?.ReservationAmenities?.Select(ra => new ReservationAmenityDto
                    {
                        AmenityName = ra.Amenity?.Name ?? string.Empty,
                        Quantity = ra.Quantity,
                        Price = ra.PriceAtTimeOfBooking
                    }).ToList() ?? new List<ReservationAmenityDto>(),
                    Payments = invoice.Payments.Select(p => new PaymentDto
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentMethod.ToString(),
                        TransactionId = p.TransactionId
                    }).ToList()
                };

                return Ok(new ApiResponse<InvoiceDto>
                {
                    Success = true,
                    Data = invoiceDto,
                    Message = "Invoice retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<InvoiceDto>
                {
                    Success = false,
                    Message = "Error retrieving invoice",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Processes a payment for an invoice
        /// </summary>
        /// <param name="id">The invoice ID to pay</param>
        /// <param name="request">Payment details including amount and payment method</param>
        /// <returns>Payment confirmation with transaction details</returns>
        /// <remarks>
        /// Partial payments are supported. The invoice status will be updated to 'Paid' when the full amount is received.
        /// Payment methods: Cash (0), CreditCard (1), DebitCard (2), BankTransfer (3)
        /// </remarks>
        // POST: api/invoices/{id}/payment
        [HttpPost("{id}/payment")]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> ProcessPayment(
            int id,
            [FromBody] ProcessPaymentRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<PaymentDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var invoice = await _context.Invoices
                    .Include(i => i.Payments)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    return NotFound(new ApiResponse<PaymentDto>
                    {
                        Success = false,
                        Message = "Invoice not found"
                    });
                }

                if (invoice.GuestId != userId)
                {
                    return Forbid();
                }

                if (invoice.Status == InvoiceStatus.Paid)
                {
                    return BadRequest(new ApiResponse<PaymentDto>
                    {
                        Success = false,
                        Message = "Invoice is already fully paid"
                    });
                }

                var result = await _paymentService.ProcessPaymentAsync(
                    id,
                    request.Amount,
                    request.PaymentMethod
                );

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<PaymentDto>
                    {
                        Success = false,
                        Message = result.Error
                    });
                }

                // Buscar o pagamento recém-criado
                var payment = await _context.Payments
                    .Where(p => p.InvoiceId == id)
                    .OrderByDescending(p => p.PaymentDate)
                    .FirstOrDefaultAsync();

                var paymentDto = payment != null ? new PaymentDto
                {
                    Id = payment.Id,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethod = payment.PaymentMethod.ToString(),
                    TransactionId = payment.TransactionId
                } : null;

                return Ok(new ApiResponse<PaymentDto>
                {
                    Success = true,
                    Data = paymentDto,
                    Message = "Payment processed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Error processing payment",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves detailed line items breakdown for an invoice
        /// </summary>
        /// <param name="id">The invoice ID</param>
        /// <returns>Itemized list of charges including room, amenities, activities, and discounts</returns>
        // GET: api/invoices/{id}/items
        [HttpGet("{id}/items")]
        public async Task<ActionResult<ApiResponse<List<InvoiceItemDto>>>> GetInvoiceItems(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<List<InvoiceItemDto>>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var invoice = await _context.Invoices
                    .Include(i => i.Reservation)
                        .ThenInclude(r => r.Room)
                    .Include(i => i.Reservation)
                        .ThenInclude(r => r.ReservationAmenities)
                            .ThenInclude(ra => ra.Amenity)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    return NotFound(new ApiResponse<List<InvoiceItemDto>>
                    {
                        Success = false,
                        Message = "Invoice not found"
                    });
                }

                if (invoice.GuestId != userId)
                {
                    return Forbid();
                }

                var items = new List<InvoiceItemDto>();

                // 1. Room charges
                if (invoice.Reservation?.Room != null)
                {
                    var nights = (invoice.Reservation.CheckOutDate - invoice.Reservation.CheckInDate).Days;

                    // Garantir que nights nunca é zero
                    if (nights <= 0) nights = 1;

                    System.Diagnostics.Debug.WriteLine($"Nights (corrected): {nights}");

                    var roomPricePerNight = invoice.Reservation.OriginalPrice.HasValue
                        ? invoice.Reservation.OriginalPrice.Value / nights
                        : invoice.Reservation.Room.PricePerNight;

                    items.Add(new InvoiceItemDto
                    {
                        Description = $"{invoice.Reservation.Room.Type} - Room {invoice.Reservation.Room.RoomNumber}",
                        UnitPrice = roomPricePerNight,
                        Quantity = nights,
                        TotalPrice = roomPricePerNight * nights
                    });
                }

                // 2. Amenities
                if (invoice.Reservation?.ReservationAmenities?.Any() == true)
                {
                    foreach (var ra in invoice.Reservation.ReservationAmenities)
                    {
                        items.Add(new InvoiceItemDto
                        {
                            Description = ra.Amenity?.Name ?? "Amenity",
                            UnitPrice = ra.PriceAtTimeOfBooking,
                            Quantity = ra.Quantity,
                            TotalPrice = ra.PriceAtTimeOfBooking * ra.Quantity
                        });
                    }
                }

                // 3. Activities
                var activities = await _context.ActivityBookings
                    .Include(ab => ab.Activity)
                    .Where(ab => ab.ReservationId == invoice.ReservationId && ab.Status == ActivityBookingStatus.Completed)
                    .ToListAsync();

                foreach (var activityBooking in activities)
                {
                    items.Add(new InvoiceItemDto
                    {
                        Description = $"{activityBooking.Activity?.Name ?? "Activity"} - {activityBooking.ScheduledDate:dd MMM yyyy}",
                        UnitPrice = activityBooking.TotalPrice / activityBooking.NumberOfPeople,
                        Quantity = activityBooking.NumberOfPeople,
                        TotalPrice = activityBooking.TotalPrice
                    });
                }

                // 4. Discount (if any promotion applied)
                if (invoice.Reservation?.DiscountPercentage.HasValue == true && invoice.Reservation.DiscountPercentage > 0)
                {
                    var discountAmount = invoice.Reservation.OriginalPrice.HasValue
                        ? (invoice.Reservation.OriginalPrice.Value - invoice.Reservation.TotalPrice)
                        : 0;

                    // Formatar percentagem sem decimais desnecessários
                    var discountPercent = invoice.Reservation.DiscountPercentage.Value % 1 == 0
                        ? invoice.Reservation.DiscountPercentage.Value.ToString("0")
                        : invoice.Reservation.DiscountPercentage.Value.ToString("0.##");

                    items.Add(new InvoiceItemDto
                    {
                        Description = $"Discount ({discountPercent}%)",
                        UnitPrice = -discountAmount,  // Negativo para dedução
                        Quantity = 1,
                        TotalPrice = -discountAmount
                    });
                }

                return Ok(new ApiResponse<List<InvoiceItemDto>>
                {
                    Success = true,
                    Data = items,
                    Message = "Invoice items retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<InvoiceItemDto>>
                {
                    Success = false,
                    Message = "Error retrieving invoice items",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
