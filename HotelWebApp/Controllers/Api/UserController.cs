using HotelWebApp.Data.Entities;
using HotelWebApp.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Manages user profile operations including viewing, updating personal information, and uploading profile photos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "ApiScheme")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public UserController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

        /// <summary>
        /// Retrieves the authenticated user's profile information
        /// </summary>
        /// <returns>User profile data including email, name, phone, and profile picture</returns>
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IdentificationDocument = user.IdentificationDocument,
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Data = userDto,
                    Message = "Profile retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Error retrieving profile",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Updates the authenticated user's profile information
        /// </summary>
        /// <param name="request">Updated profile data (full name, phone number, identification document)</param>
        /// <returns>Updated user profile</returns>
        /// <remarks>Email cannot be changed through this endpoint</remarks>
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.IdentificationDocument = request.IdentificationDocument;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "Failed to update profile",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IdentificationDocument = user.IdentificationDocument,
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Data = userDto,
                    Message = "Profile updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Error updating profile",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Uploads or updates the user's profile photo
        /// </summary>
        /// <param name="photo">Image file (JPG, PNG, or GIF, max 5MB)</param>
        /// <returns>URL of the uploaded profile photo</returns>
        /// <remarks>
        /// Replaces any existing profile photo. Accepted formats: JPG, JPEG, PNG, GIF.
        /// Maximum file size: 5MB.
        /// </remarks>
        [HttpPost("photo")]
        public async Task<ActionResult<ApiResponse<string>>> UploadPhoto(IFormFile photo)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                if (photo == null || photo.Length == 0)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No photo provided"
                    });
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Invalid file type. Only JPG, PNG and GIF are allowed."
                    });
                }

                if (photo.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "File size cannot exceed 5MB"
                    });
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // DELETAR FOTO ANTIGA (se existir)
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    // Extrai só o nome do ficheiro do caminho completo
                    var oldFileName = user.ProfilePictureUrl.Replace("/images/profiles/", "");
                    var oldFilePath = Path.Combine(_environment.WebRootPath, "images", "profiles", oldFileName);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // GUARDAR O CAMINHO COMPLETO
                var photoUrl = $"/images/profiles/{fileName}";
                user.ProfilePictureUrl = photoUrl;
                await _userManager.UpdateAsync(user);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = photoUrl,
                    Message = "Photo uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error uploading photo",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Changes the authenticated user's password
        /// </summary>
        /// <param name="request">Current password and new password</param>
        /// <returns>Confirmation of password change</returns>
        [HttpPut("change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                // Verificar password atual
                var passwordCheck = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!passwordCheck)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Current password is incorrect"
                    });
                }

                // Alterar password
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Failed to change password",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Password changed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error changing password",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
