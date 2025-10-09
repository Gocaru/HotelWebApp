using HotelWebApp.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelWebApp.Mobile.Services
{
    public class InvoiceService
    {
        private readonly HttpClient _httpClient;

        public InvoiceService(HttpClient httpClient)
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

        public async Task<ApiResponse<List<InvoiceDto>>> GetMyInvoicesAsync()
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== GET INVOICES ===");

                var response = await _httpClient.GetAsync("api/Invoices");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<InvoiceDto>>
                    {
                        Success = false,
                        Message = "Failed to load invoices"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<InvoiceDto>>>();

                System.Diagnostics.Debug.WriteLine($"Loaded {result?.Data?.Count ?? 0} invoices");

                return result ?? new ApiResponse<List<InvoiceDto>>
                {
                    Success = false,
                    Message = "Failed to parse invoices"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<List<InvoiceDto>>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<InvoiceDto>> GetInvoiceDetailsAsync(int id)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== GET INVOICE DETAILS {id} ===");

                var response = await _httpClient.GetAsync($"api/Invoices/{id}");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<InvoiceDto>
                    {
                        Success = false,
                        Message = "Failed to load invoice details"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<InvoiceDto>>();

                return result ?? new ApiResponse<InvoiceDto>
                {
                    Success = false,
                    Message = "Failed to parse invoice"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<InvoiceDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<List<InvoiceItemDto>>> GetInvoiceItemsAsync(int id)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== GET INVOICE ITEMS {id} ===");

                var response = await _httpClient.GetAsync($"api/Invoices/{id}/items");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<InvoiceItemDto>>
                    {
                        Success = false,
                        Message = "Failed to load invoice items"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<InvoiceItemDto>>>();

                System.Diagnostics.Debug.WriteLine($"Loaded {result?.Data?.Count ?? 0} items");

                return result ?? new ApiResponse<List<InvoiceItemDto>>
                {
                    Success = false,
                    Message = "Failed to parse items"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<List<InvoiceItemDto>>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }
    }
}