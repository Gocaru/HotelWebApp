using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models.Api;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Manages room reservations for guests including viewing, creating, canceling, and check-in operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "ApiScheme")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;

        public ReservationsController(
            IReservationRepository reservationRepo,
            IRoomRepository roomRepo,
            UserManager<ApplicationUser> userManager,
            IReservationService reservationService)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
            _reservationService = reservationService;
        }

        /// <summary>
        /// Retrieves all reservations for the authenticated guest
        /// </summary>
        /// <returns>Complete reservation history including room details and amenities</returns>
        // GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ReservationDto>>>> GetMyReservations()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<List<ReservationDto>>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var reservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(userId);

                var reservationDtos = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    ReservationDate = r.ReservationDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status.ToString(),
                    NumberOfGuests = r.NumberOfGuests,
                    OriginalPrice = r.OriginalPrice,
                    DiscountPercentage = r.DiscountPercentage,
                    PromotionTitle = r.Promotion?.Title,
                    Room = r.Room != null ? new RoomDto
                    {
                        Id = r.Room.Id,
                        RoomNumber = r.Room.RoomNumber,
                        Type = r.Room.Type.ToString(),
                        PricePerNight = r.Room.PricePerNight,
                        Capacity = r.Room.Capacity,
                        ImageUrl = r.Room.ImageUrl
                    } : null,
                    Amenities = r.ReservationAmenities?.Select(ra => new ReservationAmenityDto
                    {
                        AmenityName = ra.Amenity?.Name ?? string.Empty,
                        Quantity = ra.Quantity,
                        Price = ra.Amenity?.Price ?? 0
                    }).ToList() ?? new List<ReservationAmenityDto>()
                }).OrderByDescending(r => r.ReservationDate).ToList();

                return Ok(new ApiResponse<List<ReservationDto>>
                {
                    Success = true,
                    Data = reservationDtos,
                    Message = "Reservations retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ReservationDto>>
                {
                    Success = false,
                    Message = "Error retrieving reservations",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific reservation
        /// </summary>
        /// <param name="id">The reservation ID</param>
        /// <returns>Complete reservation details including room and amenities</returns>
        // GET: api/reservations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReservationDto>>> GetReservation(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Reservation not found"
                    });
                }

                if (reservation.GuestId != userId)
                {
                    return Forbid();
                }

                var reservationDto = new ReservationDto
                {
                    Id = reservation.Id,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    ReservationDate = reservation.ReservationDate,
                    TotalPrice = reservation.TotalPrice,
                    Status = reservation.Status.ToString(),
                    NumberOfGuests = reservation.NumberOfGuests,
                    OriginalPrice = reservation.OriginalPrice,
                    DiscountPercentage = reservation.DiscountPercentage,
                    PromotionTitle = reservation.Promotion?.Title,
                    Room = reservation.Room != null ? new RoomDto
                    {
                        Id = reservation.Room.Id,
                        RoomNumber = reservation.Room.RoomNumber,
                        Type = reservation.Room.Type.ToString(),
                        PricePerNight = reservation.Room.PricePerNight,
                        Capacity = reservation.Room.Capacity,
                        ImageUrl = reservation.Room.ImageUrl
                    } : null,
                    Amenities = reservation.ReservationAmenities?.Select(ra => new ReservationAmenityDto
                    {
                        AmenityName = ra.Amenity?.Name ?? string.Empty,
                        Quantity = ra.Quantity,
                        Price = ra.Amenity?.Price ?? 0
                    }).ToList() ?? new List<ReservationAmenityDto>()
                };

                return Ok(new ApiResponse<ReservationDto>
                {
                    Success = true,
                    Data = reservationDto,
                    Message = "Reservation retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ReservationDto>
                {
                    Success = false,
                    Message = "Error retrieving reservation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Cancels a confirmed reservation
        /// </summary>
        /// <param name="id">The reservation ID to cancel</param>
        /// <returns>Confirmation of cancellation</returns>
        /// <remarks>Only confirmed reservations can be cancelled, and not on or after the check-in date</remarks>
        // POST: api/reservations/{id}/cancel
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelReservation(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var reservation = await _reservationRepo.GetByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Reservation not found"
                    });
                }

                if (reservation.GuestId != userId)
                {
                    return Forbid();
                }

                if (reservation.Status != ReservationStatus.Confirmed)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Only confirmed reservations can be cancelled"
                    });
                }

                if (reservation.CheckInDate <= DateTime.Today)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Cannot cancel reservations that have already started or are for today"
                    });
                }

                reservation.Status = ReservationStatus.Cancelled;
                await _reservationRepo.UpdateAsync(reservation);

                var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
                if (room != null)
                {
                    room.Status = RoomStatus.Available;
                    await _roomRepo.UpdateAsync(room);
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Reservation cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error cancelling reservation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Performs check-in for a confirmed reservation via the mobile app
        /// </summary>
        /// <param name="id">The reservation ID to check in</param>
        /// <returns>Confirmation of check-in</returns>
        /// <remarks>Check-in is only available on the reservation date for confirmed reservations</remarks>
        // POST: api/reservations/{id}/check-in
        [HttpPost("{id}/check-in")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckIn(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Reservation not found"
                    });
                }

                if (reservation.GuestId != userId)
                {
                    return Forbid();
                }

                if (reservation.Status != ReservationStatus.Confirmed)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Only confirmed reservations can be checked in"
                    });
                }

                if (reservation.CheckInDate.Date != DateTime.Today.Date)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Check-in is only available on the reservation date"
                    });
                }

                reservation.Status = ReservationStatus.CheckedIn;
                if (reservation.Room != null)
                {
                    reservation.Room.Status = RoomStatus.Occupied;
                }

                await _reservationRepo.UpdateAsync(reservation);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Check-in completed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error during check-in",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Creates a new room reservation for the authenticated guest
        /// </summary>
        /// <param name="request">Reservation details including room, dates, and number of guests</param>
        /// <returns>The created reservation with confirmation details</returns>
        /// <remarks>
        /// Check-in time is automatically set to 12:00 PM and check-out to 11:00 AM.
        /// The room must be available for the requested dates and have sufficient capacity.
        /// </remarks>
        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReservationDto>>> CreateReservation([FromBody] CreateReservationRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                // Validar datas
                if (request.CheckInDate < DateTime.Today)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Check-in date cannot be in the past"
                    });
                }

                if (request.CheckOutDate <= request.CheckInDate)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Check-out date must be after check-in date"
                    });
                }

                // Verificar disponibilidade do quarto
                var checkInWithTime = new DateTime(request.CheckInDate.Year, request.CheckInDate.Month, request.CheckInDate.Day, 12, 0, 0);
                var checkOutWithTime = new DateTime(request.CheckOutDate.Year, request.CheckOutDate.Month, request.CheckOutDate.Day, 11, 0, 0);

                var isAvailable = await _reservationRepo.IsRoomAvailableAsync(
                    request.RoomId,
                    checkInWithTime,
                    checkOutWithTime
                );

                if (!isAvailable)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Room is not available for the selected dates"
                    });
                }

                // Verificar capacidade do quarto
                var room = await _roomRepo.GetByIdAsync(request.RoomId);
                if (room == null)
                {
                    return NotFound(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Room not found"
                    });
                }

                if (request.NumberOfGuests > room.Capacity)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = $"Room capacity is {room.Capacity} guests"
                    });
                }

                // Criar reserva usando o serviço
                var viewModel = new HotelWebApp.Models.ReservationViewModel
                {
                    GuestId = userId,
                    RoomId = request.RoomId,
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    NumberOfGuests = request.NumberOfGuests
                };

                var result = await _reservationService.CreateReservationAsync(viewModel, userId);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = result.Error
                    });
                }

                // Buscar a última reserva criada por este utilizador (a que acabou de criar)
                var userReservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(userId);
                var reservation = userReservations
                    .OrderByDescending(r => r.ReservationDate)
                    .FirstOrDefault();

                if (reservation == null)
                {
                    return StatusCode(500, new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Reservation created but could not retrieve details"
                    });
                }

                var reservationDto = new ReservationDto
                {
                    Id = reservation.Id,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    ReservationDate = reservation.ReservationDate,
                    TotalPrice = reservation.TotalPrice,
                    Status = reservation.Status.ToString(),
                    NumberOfGuests = reservation.NumberOfGuests,
                    Room = new RoomDto
                    {
                        Id = reservation.Room.Id,
                        RoomNumber = reservation.Room.RoomNumber,
                        Type = reservation.Room.Type.ToString(),
                        PricePerNight = reservation.Room.PricePerNight,
                        Capacity = reservation.Room.Capacity,
                        ImageUrl = reservation.Room.ImageUrl
                    },
                    Amenities = new List<ReservationAmenityDto>()
                };

                return Ok(new ApiResponse<ReservationDto>
                {
                    Success = true,
                    Data = reservationDto,
                    Message = "Reservation created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ReservationDto>
                {
                    Success = false,
                    Message = "Error creating reservation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
