using HotelWebApp.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Services
{
    public class ReservationService
    {
        private readonly HttpClient _httpClient;

        public ReservationService(HttpClient httpClient)
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

        public async Task<ApiResponse<List<ReservationDto>>> GetMyReservationsAsync()
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== GET RESERVATIONS ===");

                var response = await _httpClient.GetAsync("api/Reservations");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<ReservationDto>>
                    {
                        Success = false,
                        Message = "Failed to load reservations"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ReservationDto>>>();

                return result ?? new ApiResponse<List<ReservationDto>>
                {
                    Success = false,
                    Message = "Failed to parse reservations"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<List<ReservationDto>>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ReservationDto>> GetReservationDetailsAsync(int id)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== GET RESERVATION DETAILS {id} ===");

                var response = await _httpClient.GetAsync($"api/Reservations/{id}");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Failed to load reservation details"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ReservationDto>>();

                return result ?? new ApiResponse<ReservationDto>
                {
                    Success = false,
                    Message = "Failed to parse reservation"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<ReservationDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<object>> CancelReservationAsync(int id)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== CANCEL RESERVATION {id} ===");

                var response = await _httpClient.PostAsync($"api/Reservations/{id}/cancel", null);

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to cancel reservation"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                return result ?? new ApiResponse<object>
                {
                    Success = true,
                    Message = "Reservation cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<object>> CheckInReservationAsync(int id)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== CHECK-IN RESERVATION {id} ===");

                var response = await _httpClient.PostAsync($"api/Reservations/{id}/check-in", null);

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ CHECK-IN FAILED");
                    System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response: {errorContent}");

                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Failed to check-in: {response.StatusCode}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                return result ?? new ApiResponse<object>
                {
                    Success = true,
                    Message = "Check-in successful"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }
    }
}
