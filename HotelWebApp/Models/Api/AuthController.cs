using HotelWebApp.Data.Entities;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelWebApp.Models.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSender _emailSender;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Errors = new List<string> { "User not found" }
                    });
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Email not confirmed",
                        Errors = new List<string> { "Please confirm your email before logging in" }
                    });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Errors = new List<string> { "Invalid credentials" }
                    });
                }

                var isGuest = await _userManager.IsInRoleAsync(user, "Guest");
                if (!isGuest)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Access denied",
                        Errors = new List<string> { "Only guests can access the mobile app" }
                    });
                }

                var token = await _jwtTokenService.GenerateTokenAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IdentificationDocument = user.IdentificationDocument,
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Data = new LoginResponse
                    {
                        Token = token,
                        User = userDto
                    },
                    Message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize(AuthenticationSchemes = "ApiScheme")]
        [HttpPost("change-password")]
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
                        Message = "User not found",
                        Errors = new List<string> { "Invalid token" }
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

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Password change failed",
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

        [Authorize(AuthenticationSchemes = "ApiScheme")]
        [HttpPost("logout")]
        public ActionResult<ApiResponse<bool>> Logout()
        {
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Logout successful"
            });
        }
    }
}
