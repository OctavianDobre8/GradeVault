using GradeVault.Server.Models;
using GradeVault.Server.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.Text.Encodings.Web;

namespace GradeVault.Server.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user==null) 
            {
                return BadRequest("If your email exists in our system, you will receive a password reset link.");
            }

            //generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //create reset url with token 
            var encodedToken = HttpUtility.UrlEncode(token);
            var encodedEmail = HttpUtility.UrlEncode(user.Email);
            var callbackUrl = $"{Request.Scheme}://{Request.Host}/reset-password?email={encodedEmail}&token={encodedToken}";

            //log the request 
            _logger.LogInformation($"Password reset requested for  {model.Email}");

             return Ok(new { 
            message = "If your email exists in our system, you will receive a password reset link.",
            resetUrl = callbackUrl // Remove this in production, just for testing
        });

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