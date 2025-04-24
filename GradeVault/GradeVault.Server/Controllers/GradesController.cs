using System.Security.Claims;
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
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    try
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
        {
            return NotFound("No student profile found for this user.");
        }

        var grades = await _context.Grades
            .Where(g => g.StudentId == student.Id)
            .Include(g => g.Class)
            .Select(g => new GradeDTO
            {
                Id = g.Id,
                ClassName = g.Class.Name,
                Value = g.Value,
                DateAssigned = g.DateAssigned
            })
            .OrderByDescending(g => g.DateAssigned)
            .ToListAsync();

        return Ok(grades);
    }
    catch (Exception ex)
    {
        return StatusCode(500, "An error occurred while retrieving grades.");
    }
}

        [HttpGet("my-classes")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ClassDTO>>> GetMyClasses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return NotFound("No student profile found for this user.");
                }

                var classes = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id)
                    .Include(e => e.Class)
                    .ThenInclude(c => c.Teacher)
                    .Select(e => new ClassDTO
                    {
                        Id = e.Class.Id,
                        Name = e.Class.Name,
                        Subject = e.Class.Subject,
                        TeacherName = $"{e.Class.Teacher.FirstName} {e.Class.Teacher.LastName}"
                    })
                    .ToListAsync();

                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving classes.");
            }
        }

        [HttpGet("{classId}/grades")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<GradeDTO>>> GetGradesByClass(int classId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return NotFound("No student profile found for this user.");
                }

                var grades = await _context.Grades
                    .Where(g => g.StudentId == student.Id && g.ClassId == classId)
                    .Include(g => g.Class)
                    .Select(g => new GradeDTO
                    {
                        Id = g.Id,
                        ClassName = g.Class.Name,
                        Value = g.Value,
                        DateAssigned = g.DateAssigned
                    })
                    .ToListAsync();

                return Ok(grades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving grades.");
            }
        }
    }
}