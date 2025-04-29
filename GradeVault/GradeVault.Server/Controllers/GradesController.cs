using System.Security.Claims;
using GradeVault.Server.Models;
using GradeVault.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;

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

        #region Student Features
        
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
                        DateAssigned = g.DateAssigned,
                        StudentId = g.StudentId
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

                var classes = await _context.ClassEnrollments
                    .Where(e => e.StudentId == student.Id)
                    .Include(e => e.Class)
                    .ThenInclude(c => c.Teacher)
                    .Select(e => new ClassDTO
                    {
                        Id = e.Class.Id,
                        Name = e.Class.Name,
                        Description = e.Class.Description,
                        RoomNumber = e.Class.RoomNumber,
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

        [HttpGet("class/{classId}/student")]
[Authorize(Roles = "Student")]
public async Task<ActionResult<IEnumerable<GradeDTO>>> GetGradesByClassForStudent(int classId)
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

        // Verify the student is enrolled in this class
        var isEnrolled = await _context.ClassEnrollments
            .AnyAsync(e => e.ClassId == classId && e.StudentId == student.Id);

        if (!isEnrolled)
        {
            return Forbid(); // Student is not enrolled in this class
        }

        var grades = await _context.Grades
            .Where(g => g.StudentId == student.Id && g.ClassId == classId)
            .Include(g => g.Class)
            .Select(g => new GradeDTO
            {
                Id = g.Id,
                ClassName = g.Class.Name,
                Value = g.Value,
                DateAssigned = g.DateAssigned,
                StudentId = g.StudentId
            })
            .ToListAsync();

        return Ok(grades);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"An error occurred while retrieving grades: {ex.Message}");
    }
}

        
        #endregion

        #region Teacher Features
        
       [HttpGet("class/{classId}/teacher")]
[Authorize(Roles = "Teacher")]
public async Task<ActionResult<IEnumerable<GradeDTO>>> GetGradesByClassForTeacher(int classId)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    try
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (teacher == null)
        {
            return NotFound("No teacher profile found for this user.");
        }

        // Verify the class belongs to this teacher
        var classExists = await _context.Classes
            .AnyAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

        if (!classExists)
        {
            return NotFound("Class not found or you don't have permission to access it.");
        }

        var grades = await _context.Grades
            .Where(g => g.ClassId == classId)
            .Include(g => g.Student)
            .Include(g => g.Class)
            .Select(g => new GradeDTO
            {
                Id = g.Id,
                StudentId = g.StudentId,
                StudentName = g.Student.FirstName + " " + g.Student.LastName,
                ClassId = g.ClassId,
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
        return StatusCode(500, $"An error occurred while retrieving grades: {ex.Message}");
    }
}

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<GradeDTO>> CreateGrade(CreateGradeDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            // Verify the class belongs to this teacher
            var classExists = await _context.Classes
                .AnyAsync(c => c.Id == model.ClassId && c.TeacherId == teacher.Id);

            if (!classExists)
            {
                return NotFound("Class not found or you don't have permission to modify it.");
            }

            // Verify the student is enrolled in this class
            var studentEnrolled = await _context.ClassEnrollments
                .AnyAsync(e => e.ClassId == model.ClassId && e.StudentId == model.StudentId);

            if (!studentEnrolled)
            {
                return BadRequest("Student is not enrolled in this class.");
            }

            var grade = new Grade
            {
                StudentId = model.StudentId,
                ClassId = model.ClassId,
                Value = model.Value,
                DateAssigned = DateTime.UtcNow
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            var student = await _context.Students.FindAsync(model.StudentId);
            var classItem = await _context.Classes.FindAsync(model.ClassId);

            return CreatedAtAction(nameof(GetGradesByClassForTeacher), new { classId = model.ClassId }, new GradeDTO
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                StudentName = student.FirstName + " " + student.LastName,
                ClassId = grade.ClassId,
                ClassName = classItem.Name,
                Value = grade.Value,
                DateAssigned = grade.DateAssigned
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateGrade(int id, UpdateGradeDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            var grade = await _context.Grades
                .Include(g => g.Class)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
            {
                return NotFound("Grade not found.");
            }

            // Verify the class belongs to this teacher
            if (grade.Class.TeacherId != teacher.Id)
            {
                return Forbid("You don't have permission to modify this grade.");
            }

            grade.Value = model.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            var grade = await _context.Grades
                .Include(g => g.Class)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
            {
                return NotFound("Grade not found.");
            }

            // Verify the class belongs to this teacher
            if (grade.Class.TeacherId != teacher.Id)
            {
                return Forbid("You don't have permission to delete this grade.");
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("bulk-upload")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> BulkUploadGrades([FromForm] int classId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            // Verify the class belongs to this teacher
            var classExists = await _context.Classes
                .AnyAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (!classExists)
            {
                return NotFound("Class not found or you don't have permission to modify it.");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<BulkGradeUploadDTO>().ToList();
                    var newGrades = new List<Grade>();
                    var errors = new List<string>();

                    foreach (var record in records)
                    {
                        // Check if the student exists and is enrolled
                        var student = await _context.Students
                            .FirstOrDefaultAsync(s => s.Id == record.StudentId);

                        if (student == null)
                        {
                            errors.Add($"Student with ID {record.StudentId} not found.");
                            continue;
                        }

                        var enrollment = await _context.ClassEnrollments
                            .AnyAsync(e => e.StudentId == record.StudentId && e.ClassId == classId);

                        if (!enrollment)
                        {
                            errors.Add($"Student {student.FirstName} {student.LastName} is not enrolled in this class.");
                            continue;
                        }

                        // Validate grade value
                        if (record.Value < 1 || record.Value > 10)
                        {
                            errors.Add($"Invalid grade value for student {student.FirstName} {student.LastName}. Value must be between 1 and 10.");
                            continue;
                        }

                        newGrades.Add(new Grade
                        {
                            StudentId = record.StudentId,
                            ClassId = classId,
                            Value = record.Value,
                            DateAssigned = DateTime.UtcNow
                        });
                    }

                    if (errors.Count > 0)
                    {
                        return BadRequest(new { Errors = errors });
                    }

                    _context.Grades.AddRange(newGrades);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = $"{newGrades.Count} grades successfully uploaded." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the file: {ex.Message}");
            }
        }
        
        #endregion
    }
}