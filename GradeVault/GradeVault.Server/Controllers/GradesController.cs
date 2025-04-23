using GradeVault.Server.Models;
using GradeVault.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradeVault.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ApiControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public GradesController(AppDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("my-grades")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<GradeDTO>>> GetMyGrades()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            var student = await _context.Students
                                        .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound("Student profile not found for the current user.");
            }

            var grades = await _context.Grades
                                       .Where(g => g.StudentId == student.Id)
                                       .Include(g => g.Class)
                                       .Select(g => new GradeDTO
                                       {
                                           Id = g.Id,
                                           ClassName = g.Class != null ? g.Class.Name : "Unknown Class",
                                           Value = g.Value,
                                           DateAssigned = g.DateAssigned
                                       })
                                       .OrderByDescending(g => g.DateAssigned)
                                       .ToListAsync();

            return Ok(grades);
        }
    }
}