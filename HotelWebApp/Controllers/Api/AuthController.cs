using HotelWebApp.Data.Entities;
using HotelWebApp.Models.Api;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Handles authentication operations for mobile app users including login, registration, and password management
    /// </summary>
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

        /// <summary>
        /// Authenticates a guest user and returns a JWT token
        /// </summary>
        /// <param name="request">Login credentials (email and password)</param>
        /// <returns>JWT token and user profile information</returns>
        /// <remarks>Only users with the 'Guest' role can login via the mobile app</remarks>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] MobileLoginRequest request)
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

        /// <summary>
        /// Registers a new guest user and returns a JWT token
        /// </summary>
        /// <param name="request">Registration details including email, password, full name, and phone number</param>
        /// <returns>JWT token and user profile information</returns>
        /// <remarks>
        /// New users are automatically assigned the 'Guest' role and their email is confirmed by default.
        /// Password must meet the security requirements (minimum 6 characters).
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] MobileRegisterRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = new List<string> { "Email already registered" }
                    });
                }

                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                await _userManager.AddToRoleAsync(user, "Guest");

                var token = await _jwtTokenService.GenerateTokenAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
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
                    Message = "Registration successful"
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

        /// <summary>
        /// Initiates the password reset process by sending a reset token to the user's email
        /// </summary>
        /// <param name="request">Email address and app URL for the reset link</param>
        /// <returns>Success confirmation (always returns success for security reasons)</returns>
        /// <remarks>
        /// For security, this endpoint always returns success even if the email doesn't exist.
        /// The reset token is sent via email and can be used in the reset-password endpoint.
        /// </remarks>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] MobileForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true,
                        Message = "If the email exists, a password reset link has been sent"
                    });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = $"{request.AppUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset Your Password",
                    $"Please reset your password by clicking here: <a href='{resetLink}'>Reset Password</a><br/><br/>Or use this code in the app: {token}"
                );

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "If the email exists, a password reset link has been sent"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error processing request",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Resets the user's password using a valid reset token
        /// </summary>
        /// <param name="request">Email, reset token, and new password</param>
        /// <returns>Success confirmation</returns>
        /// <remarks>The reset token must be obtained from the forgot-password endpoint</remarks>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] MobileResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request",
                        Errors = new List<string> { "User not found" }
                    });
                }

                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Password reset failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Password reset successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error resetting password",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Changes the password for an authenticated user
        /// </summary>
        /// <param name="request">Current password and new password</param>
        /// <returns>Success confirmation</returns>
        /// <remarks>Requires authentication. User must provide their current password to change it.</remarks>
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

        /// <summary>
        /// Logs out the authenticated user
        /// </summary>
        /// <returns>Success confirmation</returns>
        /// <remarks>
        /// For JWT-based authentication, logout is handled client-side by discarding the token.
        /// This endpoint exists for consistency and to allow server-side tracking if needed.
        /// </remarks>
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
