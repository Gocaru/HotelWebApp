using HotelWebApp.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelWebApp.Mobile.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task AddAuthorizationHeaderAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse<PaymentDto>> ProcessPaymentAsync(MobilePaymentRequest request)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== PROCESS PAYMENT ===");
                System.Diagnostics.Debug.WriteLine($"InvoiceId: {request.InvoiceId}");
                System.Diagnostics.Debug.WriteLine($"Amount: {request.Amount}");

                var response = await _httpClient.PostAsJsonAsync("api/MobilePayments/process", request);

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error: {errorContent}");

                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentDto>>();
                        return errorResponse ?? new ApiResponse<PaymentDto>
                        {
                            Success = false,
                            Message = "Payment failed"
                        };
                    }
                    catch
                    {
                        return new ApiResponse<PaymentDto>
                        {
                            Success = false,
                            Message = $"Payment failed: {response.StatusCode}"
                        };
                    }
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentDto>>();

                System.Diagnostics.Debug.WriteLine($"Payment Success: {result?.Success}");

                return result ?? new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = "Failed to parse payment response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<PaymentDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }
    }
}