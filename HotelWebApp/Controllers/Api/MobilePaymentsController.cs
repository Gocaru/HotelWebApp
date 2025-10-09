using HotelWebApp.Models.Api;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// API Controller for processing payments from mobile app
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "ApiScheme")]
    public class MobilePaymentsController : ControllerBase
    {
        private readonly IMobilePaymentService _mobilePaymentService;

        public MobilePaymentsController(IMobilePaymentService mobilePaymentService)
        {
            _mobilePaymentService = mobilePaymentService;
        }

        /// <summary>
        /// Processes a payment with card details from mobile app
        /// </summary>
        /// <param name="request">Payment details including card information</param>
        /// <returns>Payment confirmation with transaction ID</returns>
        // POST: api/MobilePayments/process
        [HttpPost("process")]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> ProcessPayment([FromBody] MobilePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Invalid payment data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "User not found in token"
                });
            }

            try
            {
                var result = await _mobilePaymentService.ProcessMobilePaymentAsync(request, userId);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Payment processing error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}