using HotelWebApp.Models.Api;

namespace HotelWebApp.Services
{
    public interface IMobilePaymentService
    {
        Task<ApiResponse<PaymentDto>> ProcessMobilePaymentAsync(MobilePaymentRequest request, string userId);
    }
}