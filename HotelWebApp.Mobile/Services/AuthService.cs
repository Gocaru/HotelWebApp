using HotelWebApp.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelWebApp.Mobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string? _authToken;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);

        public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LOGIN REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"BaseAddress: {_httpClient.BaseAddress}");
                System.Diagnostics.Debug.WriteLine($"Timeout: {_httpClient.Timeout}");
                System.Diagnostics.Debug.WriteLine($"Email: {email}");

                var request = new MobileLoginRequest
                {
                    Email = email,
                    Password = password
                };

                System.Diagnostics.Debug.WriteLine($"⏱️ Starting HTTP POST at {DateTime.Now:HH:mm:ss.fff}");

                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);

                System.Diagnostics.Debug.WriteLine($"✅ HTTP POST completed at {DateTime.Now:HH:mm:ss.fff}");
                System.Diagnostics.Debug.WriteLine($"AuthService: StatusCode = {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error Response: {errorContent}");

                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
                        return errorResponse ?? new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = "Invalid email or password"
                        };
                    }
                    catch
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = response.StatusCode == System.Net.HttpStatusCode.BadRequest
                                ? "Invalid email or password"
                                : $"Connection error: {response.StatusCode}"
                        };
                    }
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

                System.Diagnostics.Debug.WriteLine($"API Response - Success: {result?.Success}");
                System.Diagnostics.Debug.WriteLine($"API Response - Message: {result?.Message}");

                if (result?.Success == true && result.Data != null)
                {
                    _authToken = result.Data.Token;
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _authToken);

                    // Guardar token localmente
                    await SecureStorage.SetAsync("auth_token", _authToken);
                    System.Diagnostics.Debug.WriteLine("✅ Login successful, token saved");
                }

                return result ?? new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Unknown error"
                };
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ TIMEOUT at {DateTime.Now:HH:mm:ss.fff}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Connection timeout - check if API is accessible"
                };
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HTTP ERROR at {DateTime.Now:HH:mm:ss.fff}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = $"Network error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GENERAL EXCEPTION at {DateTime.Now:HH:mm:ss.fff}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = $"Connection error: { ex.Message }"
                };
            }
        }

        public async Task<bool> LoadTokenAsync()
        {
            try
            {
                _authToken = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(_authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _authToken);
                    return true;
                }
            }
            catch { }

            return false;
        }

        public async Task LogoutAsync()
        {
            _authToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");
        }

        public string? GetToken() => _authToken;
    }
}
