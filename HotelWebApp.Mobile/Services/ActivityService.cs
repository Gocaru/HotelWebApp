using HotelWebApp.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelWebApp.Mobile.Services
{
    public class ActivityService
    {
        private readonly HttpClient _httpClient;

        public ActivityService(HttpClient httpClient)
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

        public async Task<ApiResponse<List<ActivityDto>>> GetActivitiesAsync()
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== GET ACTIVITIES ===");

                var response = await _httpClient.GetAsync("api/Activities");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ERROR: {errorContent}");

                    return new ApiResponse<List<ActivityDto>>
                    {
                        Success = false,
                        Message = "Failed to load activities"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response: {content.Substring(0, Math.Min(200, content.Length))}...");

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ActivityDto>>>();

                if (result?.Success == true && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Loaded {result.Data.Count} activities");
                }

                return result ?? new ApiResponse<List<ActivityDto>>
                {
                    Success = false,
                    Message = "Failed to parse activities"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return new ApiResponse<List<ActivityDto>>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ActivityDto>> GetActivityByIdAsync(int id)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== GET ACTIVITY DETAILS {id} ===");

                var response = await _httpClient.GetAsync($"api/Activities/{id}");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<ActivityDto>
                    {
                        Success = false,
                        Message = "Failed to load activity details"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();

                return result ?? new ApiResponse<ActivityDto>
                {
                    Success = false,
                    Message = "Failed to parse activity"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<ActivityDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ActivityAvailabilityDto>> CheckAvailabilityAsync(int activityId, DateTime date)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== CHECK ACTIVITY AVAILABILITY {activityId} for {date:yyyy-MM-dd} ===");

                // ✅ API espera: GET /api/Activities/{id}/availability?date=2025-01-15
                var response = await _httpClient.GetAsync(
                    $"api/Activities/{activityId}/availability?date={date:yyyy-MM-dd}");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ERROR: {errorContent}");

                    return new ApiResponse<ActivityAvailabilityDto>
                    {
                        Success = false,
                        Message = "Failed to check availability"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityAvailabilityDto>>();

                if (result?.Success == true && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Available: {result.Data.AvailableSpots}/{result.Data.Capacity} spots");
                }

                return result ?? new ApiResponse<ActivityAvailabilityDto>
                {
                    Success = false,
                    Message = "Failed to parse availability"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<ActivityAvailabilityDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ActivityBookingDto>> BookActivityAsync(CreateActivityBookingRequest request)
        {
            System.Diagnostics.Debug.WriteLine($" SERVICE - Before HTTP Call");
            System.Diagnostics.Debug.WriteLine($"   request.ScheduledDate: {request.ScheduledDate:yyyy-MM-dd HH:mm:ss zzz}");
            System.Diagnostics.Debug.WriteLine($"   request.ScheduledDate.Kind: {request.ScheduledDate.Kind}");

            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== BOOK ACTIVITY ===");
                System.Diagnostics.Debug.WriteLine($"ActivityId: {request.ActivityId}");
                System.Diagnostics.Debug.WriteLine($"ScheduledDate: {request.ScheduledDate}");
                System.Diagnostics.Debug.WriteLine($"NumberOfPeople: {request.NumberOfPeople}");
                System.Diagnostics.Debug.WriteLine($"ReservationId: {request.ReservationId}");

                // API espera POST para /api/Activities/{id}/book
                var response = await _httpClient.PostAsJsonAsync(
                    $"api/Activities/{request.ActivityId}/book",
                    new
                    {
                        ScheduledDate = request.ScheduledDate,
                        NumberOfPeople = request.NumberOfPeople,
                        ReservationId = request.ReservationId
                    });

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode} ({(int)response.StatusCode})");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ERROR Content: {errorContent}");

                    // Mostrar erro real no mobile
                    return new ApiResponse<ActivityBookingDto>
                    {
                        Success = false,
                        Message = $"HTTP {(int)response.StatusCode}: {errorContent.Substring(0, Math.Min(300, errorContent.Length))}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityBookingDto>>();

                return result ?? new ApiResponse<ActivityBookingDto>
                {
                    Success = false,
                    Message = "Failed to parse booking"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<ActivityBookingDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<List<ActivityBookingDto>>> GetMyBookingsAsync()
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== GET MY ACTIVITY BOOKINGS ===");

                var response = await _httpClient.GetAsync("api/Activities/my-bookings");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<List<ActivityBookingDto>>
                    {
                        Success = false,
                        Message = "Failed to load bookings"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ActivityBookingDto>>>();

                return result ?? new ApiResponse<List<ActivityBookingDto>>
                {
                    Success = false,
                    Message = "Failed to parse bookings"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<List<ActivityBookingDto>>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<object>> CancelBookingAsync(int bookingId)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== CANCEL ACTIVITY BOOKING {bookingId} ===");

                var response = await _httpClient.DeleteAsync($"api/Activities/bookings/{bookingId}");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to cancel booking"
                    };
                }

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Booking cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }
    }
}