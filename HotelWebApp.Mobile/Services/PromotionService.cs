using HotelWebApp.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelWebApp.Mobile.Services;

public class PromotionService
{
    private readonly HttpClient _httpClient;

    public PromotionService(HttpClient httpClient)
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

    public async Task<ApiResponse<List<PromotionDto>>> GetPromotionsAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            System.Diagnostics.Debug.WriteLine("=== GET PROMOTIONS ===");

            var response = await _httpClient.GetAsync("api/Promotions");

            System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"ERROR: {errorContent}");

                return new ApiResponse<List<PromotionDto>>
                {
                    Success = false,
                    Message = "Failed to load promotions"
                };
            }

            var content = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response: {content}");

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PromotionDto>>>();

            if (result?.Success == true && result.Data != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Loaded {result.Data.Count} promotions");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Failed: {result?.Message}");
            }

            return result ?? new ApiResponse<List<PromotionDto>>
            {
                Success = false,
                Message = "Failed to parse promotions"
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}"); 

            return new ApiResponse<List<PromotionDto>>
            {
                Success = false,
                Message = $"Connection error: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<PromotionDto>> GetPromotionByIdAsync(int id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            System.Diagnostics.Debug.WriteLine($"=== GET PROMOTION DETAILS {id} ===");

            var response = await _httpClient.GetAsync($"api/Promotions/{id}");

            System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<PromotionDto>
                {
                    Success = false,
                    Message = "Failed to load promotion details"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PromotionDto>>();

            return result ?? new ApiResponse<PromotionDto>
            {
                Success = false,
                Message = "Failed to parse promotion"
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            return new ApiResponse<PromotionDto>
            {
                Success = false,
                Message = $"Connection error: {ex.Message}"
            };
        }
    }
}

