using GradeVault.Server.Models;
using GradeVault.Server.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.Text.Encodings.Web;
using GradeVault.Server.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GradeVault.Server.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = Log.ForContext<AuthController>();
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Password is required.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.Information("User {Email} logged in successfully", model.Email);
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                });
            }

            if (result.IsLockedOut)
            {
                _logger.Warning("User account {Email} locked out", model.Email);
                return StatusCode(StatusCodes.Status423Locked, "Account is locked. Please try again later.");
            }

            _logger.Warning("Invalid login attempt for {Email}", model.Email);
            return Unauthorized("Invalid login attempt.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            try
            {
                _logger.Debug("Received registration request for email: {Email}", model.Email);
                
                if (!ModelState.IsValid)
                {
                    _logger.Warning("Invalid model state for registration request: {@ModelErrors}", 
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    _logger.Warning("Missing password or confirmation password");
                    return BadRequest("Password and Confirm Password are required.");
                }

                if (model.Password != model.ConfirmPassword)
                {
                    _logger.Warning("Passwords do not match for email: {Email}", model.Email);
                    return BadRequest("Passwords do not match");
                }

                if (string.IsNullOrEmpty(model.Role))
                {
                    _logger.Warning("Role not specified for email: {Email}", model.Email);
                    return BadRequest("Role is required.");
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.Warning("Email {Email} is already registered", model.Email);
                    return BadRequest("Email is already registered.");
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.Role
                };

                _logger.Information("Attempting to create user with email: {Email}", model.Email);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.Information("User created successfully. Adding to role: {Role}", model.Role);
                    try
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                        
                        if (!roleResult.Succeeded)
                        {
                            _logger.Error("Failed to add user to role {Role}. Errors: {@Errors}", 
                                model.Role, 
                                roleResult.Errors.Select(e => e.Description));
                            
                            // Clean up the user since role assignment failed
                            await _userManager.DeleteAsync(user);
                            return BadRequest($"Failed to assign role {model.Role}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error occurred while adding user {Email} to role {Role}", model.Email, model.Role);
                        await _userManager.DeleteAsync(user);
                        return StatusCode(500, $"Role assignment failed: {ex.Message}");
                    }

                    try
                    {
                        _logger.Debug("Getting database context for creating {Role} record", model.Role);
                        var dbContext = HttpContext.RequestServices.GetRequiredService<AppDatabaseContext>();
                        
                        if (model.Role == "Student")
                        {
                            _logger.Information("Creating Student record for user {UserId} with email {Email}", user.Id, user.Email);
                            var student = new Student
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                UserId = user.Id
                            };

                            dbContext.Students.Add(student);
                            await dbContext.SaveChangesAsync();
                            _logger.Debug("Student record created successfully");
                        }
                        else if (model.Role == "Teacher")
                        {
                            _logger.Information("Creating Teacher record for user {UserId} with email {Email}", user.Id, user.Email);
                            var teacher = new Teacher
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                UserId = user.Id
                            };

                            dbContext.Teachers.Add(teacher);
                            await dbContext.SaveChangesAsync();
                            _logger.Debug("Teacher record created successfully");
                        }
                        
                        _logger.Information("Registration successful for {Email} with role {Role}", model.Email, model.Role);
                        return Ok(new { message = "Registration successful" });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Database error when creating {Role} record for user {UserId} with email {Email}", 
                            model.Role, user.Id, user.Email);
                        // Clean up the created user since entity creation failed
                        await _userManager.DeleteAsync(user);
                        return StatusCode(500, $"User profile creation failed: {ex.Message}");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.Warning("User creation error for {Email}: {Error}", model.Email, error.Description);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled exception during user registration for email {Email}", model.Email ?? "unknown");
                return StatusCode(500, $"Registration failed due to an unexpected error: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for forgot password request");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                _logger.Information("Forgot password requested for non-existent email: {Email}", model.Email);
                return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
            }

            _logger.Information("Generating password reset token for user {Email}", model.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            var encodedToken = HttpUtility.UrlEncode(token);
            var encodedEmail = HttpUtility.UrlEncode(model.Email);
            
            var callbackUrl = $"{Request.Scheme}://{Request.Host}/reset-password?email={encodedEmail}&token={encodedToken}";
            
            string emailSubject = "Reset Your Password";
            string emailBody = $@"
                <h2>Reset Your GradeVault Password</h2>
                <p>Hello {user.FirstName},</p>
                <p>Please reset your password by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.</p>
                <p>If you didn't request this change, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
            ";

            try
            {
                _logger.Debug("Sending password reset email to {Email}", model.Email);
                await _emailService.SendEmailAsync(model.Email, emailSubject, emailBody);
                _logger.Information("Password reset email sent to {Email}", model.Email);
                
                return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending password reset email to {Email}", model.Email);
                return StatusCode(500, "Error sending password reset email. Please try again later.");
            }
        }

        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                _logger.Warning("Missing email or token in reset token validation request");
                return BadRequest("Email and token are required");
            }

            _logger.Debug("Validating reset token for {Email}", email);
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                _logger.Warning("Reset token validation attempted for non-existent user {Email}", email);
                return BadRequest("Invalid token");
            }

            var isValid = await _userManager.VerifyUserTokenAsync(
                user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<User>.ResetPasswordTokenPurpose,
                token);

            if (!isValid)
            {
                _logger.Warning("Invalid or expired reset token for user {Email}", email);
                return BadRequest("Invalid or expired token");
            }

            _logger.Information("Password reset token validated successfully for {Email}", email);
            return Ok(new { isValid = true });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for password reset request");
                return BadRequest(ModelState);
            }

            _logger.Debug("Processing password reset for {Email}", model.Email);
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                _logger.Warning("Password reset attempted for non-existent user {Email}", model.Email);
                return BadRequest("Failed to reset password");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            
            if (result.Succeeded)
            {
                _logger.Information("Password reset successful for user {Email}", model.Email);
                
                string emailSubject = "Your Password Has Been Reset";
                string emailBody = $@"
                    <h2>Password Reset Confirmation</h2>
                    <p>Hello {user.FirstName},</p>
                    <p>Your password for GradeVault has been successfully reset.</p>
                    <p>If you didn't make this change, please contact support immediately.</p>
                ";
                
                try
                {
                    _logger.Debug("Sending password reset confirmation email to {Email}", model.Email);
                    await _emailService.SendEmailAsync(model.Email, emailSubject, emailBody);
                    _logger.Debug("Password reset confirmation email sent to {Email}", model.Email);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error sending password reset confirmation email to {Email}", model.Email);
                    // Continue even if confirmation email fails
                }
                
                return Ok(new { message = "Password has been reset successfully." });
            }

            _logger.Warning("Password reset failed for {Email}: {Errors}", 
                model.Email, 
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.Information("User logged out");
            return Ok(new { message = "Logged out successfully" });
        }
    }
}