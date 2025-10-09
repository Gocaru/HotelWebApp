using HotelWebApp.Data.Entities;
using HotelWebApp.Models.Api;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IEmailConfirmationService _emailConfirmationService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            IEmailSender emailSender,
            IEmailConfirmationService emailConfirmationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
            _emailConfirmationService = emailConfirmationService;
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
        /// Registers a new guest user and sends email confirmation
        /// </summary>
        /// <param name="request">Registration details including email, password, full name, and phone number</param>
        /// <returns>Success message instructing user to check email</returns>
        /// <remarks>
        /// New users are automatically assigned the 'Guest' role but email must be confirmed before login.
        /// Password must meet the security requirements (minimum 6 characters).
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> Register([FromBody] MobileRegisterRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== REGISTER REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");

                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new ApiResponse<bool>
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
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                await _userManager.AddToRoleAsync(user, "Guest");

                // Gera código de 6 dígitos
                var code = _emailConfirmationService.GenerateAndStoreCode(user.Id);
                System.Diagnostics.Debug.WriteLine($"6-digit code generated: {code}");

                // Envia mail com o código
                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirm Your Email - HotelWebApp",
                        $@"
                        <h2>Welcome to HotelWebApp!</h2>
    
                        <p>Thank you for registering, <strong>{user.FullName}</strong>.</p>
    
                        <p>Your email confirmation code is:</p>
    
                        <h1 style='background-color: #6366F1; color: white; padding: 20px; text-align: center; font-size: 36px; letter-spacing: 5px;'>
                            {code}
                        </h1>
    
                        <p>This code will expire in 15 minutes.</p>
    
                        <p>If you didn't create this account, please ignore this email.</p>
    
                        <p>---<br>HotelWebApp Team</p>
                        ");

                    System.Diagnostics.Debug.WriteLine("Confirmation email sent successfully!");
                }
                catch (Exception emailEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Email sending failed: {emailEx.Message}");

                    await _userManager.DeleteAsync(user);

                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Failed to send confirmation email. Please try again.",
                        Errors = new List<string> { emailEx.Message }
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Registration successful! Check your email for a 6-digit confirmation code."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Confirms user email with the provided token
        /// </summary>
        /// <param name="request">Email and confirmation token</param>
        /// <returns>Success confirmation</returns>
        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== CONFIRM EMAIL REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");
                System.Diagnostics.Debug.WriteLine($"Code: {request.Token}");

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

                if (user.EmailConfirmed)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true,
                        Message = "Email already confirmed. You can login now."
                    });
                }

                // VALIDAR CÓDIGO DE 6 DÍGITOS
                var isValid = _emailConfirmationService.ValidateCode(user.Id, request.Token);

                if (!isValid)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Invalid or expired code");
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid or expired confirmation code",
                        Errors = new List<string> { "Please request a new code" }
                    });
                }

                // CONFIRMAR EMAIL
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                System.Diagnostics.Debug.WriteLine("✅ Email confirmed successfully!");

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Email confirmed successfully! You can now login."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ EXCEPTION: {ex.Message}");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error confirming email",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Resends the email confirmation code
        /// </summary>
        /// <param name="request">User email</param>
        /// <returns>Success confirmation</returns>
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ResendConfirmationEmail([FromBody] ResendConfirmationRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== RESEND CONFIRMATION EMAIL ===");
                System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");

                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true,
                        Message = "If the email exists, a new confirmation code has been sent."
                    });
                }

                if (user.EmailConfirmed)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true,
                        Message = "Email already confirmed. You can login now."
                    });
                }

                // GERAR NOVO CÓDIGO
                var code = _emailConfirmationService.GenerateAndStoreCode(user.Id);
                System.Diagnostics.Debug.WriteLine($"New 6-digit code: {code}");

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "New Confirmation Code - HotelWebApp",
                    $@"<html>
                <body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; padding: 30px;'>
                        <h2 style='color: #6366F1;'>New Confirmation Code</h2>
                        <p>Hi <strong>{user.FullName}</strong>,</p>
                        <p>Here's your new confirmation code:</p>
                        
                        <div style='background: linear-gradient(135deg, #6366F1 0%, #8B5CF6 100%); color: white; padding: 30px; border-radius: 10px; text-align: center; margin: 30px 0;'>
                            <h1 style='margin: 0; letter-spacing: 8px; font-size: 48px;'>{code}</h1>
                        </div>
                        
                        <p style='color: #666; font-size: 14px;'>This code will expire in <strong>15 minutes</strong>.</p>
                    </div>
                </body>
              </html>");

                System.Diagnostics.Debug.WriteLine("Confirmation email resent!");

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "A new confirmation code has been sent to your email."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error sending confirmation email",
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
                System.Diagnostics.Debug.WriteLine($"=== FORGOT PASSWORD REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"Email: {request.Email}");

                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("User not found");
                    return Ok(new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true,
                        Message = "If the email exists, a password reset link has been sent"
                    });
                }

                System.Diagnostics.Debug.WriteLine($"User found: {user.Email}");
                System.Diagnostics.Debug.WriteLine($"EmailConfirmed: {user.EmailConfirmed}");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                System.Diagnostics.Debug.WriteLine($"Token generated (length: {token.Length})");

                var resetLink = $"{request.AppUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                System.Diagnostics.Debug.WriteLine($"🔗 Reset Link: {resetLink}");
                System.Diagnostics.Debug.WriteLine($"📧 Sending email to: {user.Email}");

                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Reset Your Password",
                        $"Please reset your password by clicking here: <a href='{resetLink}'>Reset Password</a><br/><br/>Or use this code in the app: {token}"
                    );

                    System.Diagnostics.Debug.WriteLine("✅ Email sent successfully!");
                }
                catch (Exception emailEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Email sending failed: {emailEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {emailEx.StackTrace}");
                    throw; // Re-throw para ver o erro completo
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "If the email exists, a password reset link has been sent"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

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
