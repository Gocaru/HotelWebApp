using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Models.Api;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Handles payment processing for mobile app with card validation simulation
    /// </summary>
    public class MobilePaymentService : IMobilePaymentService
    {
        private readonly HotelWebAppContext _context;
        private readonly IPaymentService _paymentService;

        public MobilePaymentService(HotelWebAppContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<ApiResponse<PaymentDto>> ProcessMobilePaymentAsync(MobilePaymentRequest request, string userId)
        {
            // Validate invoice belongs to user
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && i.GuestId == userId);

            if (invoice == null)
            {
                return new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Invoice not found"
                };
            }

            // Check if already paid
            if (invoice.Status == InvoiceStatus.Paid)
            {
                return new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Invoice is already fully paid"
                };
            }

            // Calculate remaining amount
            var paidAmount = invoice.Payments?.Sum(p => p.Amount) ?? 0;
            var remainingAmount = invoice.TotalAmount - paidAmount;

            if (request.Amount > remainingAmount)
            {
                return new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = $"Payment amount exceeds remaining balance of €{remainingAmount:F2}"
                };
            }

            // Simulate payment gateway processing
            var (isSuccess, transactionId) = SimulatePaymentGateway(request);

            if (!isSuccess)
            {
                return new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Payment failed. Please check your card details and try again."
                };
            }

            // Parse payment method
            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, out var paymentMethodEnum))
            {
                paymentMethodEnum = PaymentMethod.CreditCard;
            }

            // Use existing PaymentService to process the payment
            var result = await _paymentService.ProcessPaymentAsync(
                request.InvoiceId,
                request.Amount,
                paymentMethodEnum
            );

            if (!result.Succeeded)
            {
                return new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = result.Error ?? "Payment processing failed"
                };
            }

            // Get the newly created payment
            var payment = await _context.Payments
                .Where(p => p.InvoiceId == request.InvoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefaultAsync();

            if (payment != null)
            {
                // Update transaction ID from gateway simulation
                payment.TransactionId = transactionId;
                await _context.SaveChangesAsync();
            }

            var paymentDto = payment != null ? new PaymentDto
            {
                Id = payment.Id,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod.ToString(),
                TransactionId = payment.TransactionId
            } : null;

            return new ApiResponse<PaymentDto>
            {
                Success = true,
                Data = paymentDto,
                Message = "Payment processed successfully"
            };
        }

        private (bool isSuccess, string transactionId) SimulatePaymentGateway(MobilePaymentRequest request)
        {
            // Generate transaction ID
            var transactionId = $"TXN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";

            // Simulate payment processing delay
            Thread.Sleep(1500);

            // Simulate success/failure (90% success rate)
            // For testing: reject cards ending in "0000"
            if (request.CardNumber.EndsWith("0000"))
            {
                return (false, transactionId);
            }

            bool success = Random.Shared.Next(100) < 90;
            return (success, transactionId);
        }
    }
}