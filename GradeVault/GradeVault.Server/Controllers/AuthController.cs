using GradeVault.Server.Models;
using GradeVault.Server.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.Text.Encodings.Web;
using GradeVault.Server.Services;

namespace GradeVault.Server.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthController> logger,IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
                _logger.LogInformation("User logged in.");
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
                _logger.LogWarning("User account locked out.");
                return StatusCode(StatusCodes.Status423Locked, "Account is locked. Please try again later.");
            }

            return Unauthorized("Invalid login attempt.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                return BadRequest("Password and Confirm Password are required.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }

            if (string.IsNullOrEmpty(model.Role))
            {
                return BadRequest("Role is required.");
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                await _userManager.AddToRoleAsync(user, model.Role);

                if (model.Role == "Student")
                {
                    var student = new Student
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserId = user.Id
                    };

                    var dbContext = HttpContext.RequestServices.GetRequiredService<AppDatabaseContext>();
                    dbContext.Students.Add(student);
                    await dbContext.SaveChangesAsync();
                }
                else if (model.Role == "Teacher")
                {
                    var teacher = new Teacher
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserId = user.Id
                    };

                    var dbContext = HttpContext.RequestServices.GetRequiredService<AppDatabaseContext>();
                    dbContext.Teachers.Add(teacher);
                    await dbContext.SaveChangesAsync();
                }

                return Ok(new { message = "Registration successful" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var user = await _userManager.FindByEmailAsync(model.Email);
    
    // Don't reveal that the user does not exist
    if (user == null)
    {
        return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
    }

    // Generate password reset token
    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    
    // Encode token for URL
    var encodedToken = HttpUtility.UrlEncode(token);
    var encodedEmail = HttpUtility.UrlEncode(model.Email);
    
    // Build password reset link
    var callbackUrl = $"{Request.Scheme}://{Request.Host}/reset-password?email={encodedEmail}&token={encodedToken}";
    
    // Prepare email content
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
        // Send email with password reset link
        await _emailService.SendEmailAsync(model.Email, emailSubject, emailBody);
        
        return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error sending password reset email to {model.Email}");
        return StatusCode(500, "Error sending password reset email. Please try again later.");
    }
}


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return Ok(new { message = "Logged out successfully" });
        }

    }
}