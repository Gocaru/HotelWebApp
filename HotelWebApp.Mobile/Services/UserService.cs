using HotelWebApp.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelWebApp.Mobile.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
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

        public async Task<ApiResponse<UserDto>> GetProfileAsync()
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== GET PROFILE ===");

                var response = await _httpClient.GetAsync("api/User/profile");

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "Failed to load profile"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

                if (result?.Success == true && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Profile loaded: {result.Data.FullName}");
                }

                return result ?? new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Failed to parse profile"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== UPDATE PROFILE ===");
                System.Diagnostics.Debug.WriteLine($"FullName: {request.FullName}");

                var response = await _httpClient.PutAsJsonAsync("api/User/profile", request);

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ERROR: {errorContent}");

                    return new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "Failed to update profile"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

                return result ?? new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Failed to parse response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<string>> UploadProfilePhotoAsync(Stream imageStream, string fileName)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine($"=== UPLOAD PROFILE PHOTO: {fileName} ===");

                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(imageStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "photo", fileName);

                var response = await _httpClient.PostAsync("api/User/photo", content);

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ERROR: {errorContent}");

                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Failed to upload photo"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

                if (result?.Success == true && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Photo uploaded: {result.Data}");
                }

                return result ?? new ApiResponse<string>
                {
                    Success = false,
                    Message = "Failed to parse response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                await AddAuthorizationHeaderAsync();

                System.Diagnostics.Debug.WriteLine("=== CHANGE PASSWORD ===");

                var response = await _httpClient.PutAsJsonAsync("api/User/change-password", request);

                System.Diagnostics.Debug.WriteLine($"StatusCode: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ERROR: {errorContent}");

                    // Tentar parsear mensagem de erro
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                        return errorResponse ?? new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Failed to change password"
                        };
                    }
                    catch
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Failed to change password"
                        };
                    }
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

                if (result?.Success == true)
                {
                    System.Diagnostics.Debug.WriteLine("Password changed successfully");
                }

                return result ?? new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to parse response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }
    }
}
