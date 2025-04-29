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
    /// <summary>
    /// Controller for managing grade-related operations in GradeVault.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for both students and teachers to interact with grades,
    /// including viewing grades, creating new grades, updating existing grades, and bulk importing
    /// grade data. Access to functionality is appropriately limited based on user roles.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ApiControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the GradesController.
        /// </summary>
        /// <param name="context">Database context for data access operations.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager for user operations.</param>
        public GradesController(AppDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region Student Features
        
        /// <summary>
        /// Retrieves all grades for the currently authenticated student.
        /// </summary>
        /// <returns>Collection of grades for the student across all classes.</returns>
        /// <remarks>
        /// This endpoint retrieves all grades assigned to the authenticated student
        /// across all classes they are enrolled in, ordered by most recent first.
        /// </remarks>
        /// <response code="200">Returns the list of grades.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no student profile is found for the authenticated user.</response>
        /// <response code="500">If a server error occurs while retrieving the data.</response>
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

        /// <summary>
        /// Retrieves all classes the currently authenticated student is enrolled in.
        /// </summary>
        /// <returns>Collection of classes the student is enrolled in.</returns>
        /// <remarks>
        /// This endpoint returns all classes where the authenticated student has an active
        /// enrollment record, including teacher information for each class.
        /// </remarks>
        /// <response code="200">Returns the list of enrolled classes.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no student profile is found for the authenticated user.</response>
        /// <response code="500">If a server error occurs while retrieving the data.</response>
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

        /// <summary>
        /// Retrieves all grades for the authenticated student in a specific class.
        /// </summary>
        /// <param name="classId">ID of the class to get grades from.</param>
        /// <returns>Collection of student grades for the specified class.</returns>
        /// <remarks>
        /// This endpoint verifies that the student is enrolled in the specified class
        /// before returning their grades for that class.
        /// </remarks>
        /// <response code="200">Returns the list of grades for the specific class.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the student is not enrolled in the specified class.</response>
        /// <response code="404">If no student profile is found for the authenticated user.</response>
        /// <response code="500">If a server error occurs while retrieving the data.</response>
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
        
        /// <summary>
        /// Retrieves all grades for all students in a specific class.
        /// </summary>
        /// <param name="classId">ID of the class to get grades from.</param>
        /// <returns>Collection of all grades in the specified class.</returns>
        /// <remarks>
        /// This endpoint verifies that the authenticated teacher is the owner of the specified class
        /// before returning all grades assigned within that class.
        /// </remarks>
        /// <response code="200">Returns the list of all grades for the class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no teacher profile is found, or the class is not found, or the teacher does not own the class.</response>
        /// <response code="500">If a server error occurs while retrieving the data.</response>
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

        /// <summary>
        /// Creates a new grade for a student in a class.
        /// </summary>
        /// <param name="model">Grade creation data transfer object.</param>
        /// <returns>Newly created grade details.</returns>
        /// <remarks>
        /// This endpoint allows teachers to assign a new grade to a student in one of their classes.
        /// It validates that the class belongs to the teacher and that the student is enrolled in the class.
        /// </remarks>
        /// <response code="201">Returns the newly created grade.</response>
        /// <response code="400">If the student is not enrolled in the class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no teacher profile is found, or the class is not found, or the teacher does not own the class.</response>
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

        /// <summary>
        /// Updates an existing grade's value.
        /// </summary>
        /// <param name="id">ID of the grade to update.</param>
        /// <param name="model">Grade update data transfer object.</param>
        /// <returns>No content on successful update.</returns>
        /// <remarks>
        /// This endpoint allows teachers to modify the value of an existing grade.
        /// It verifies that the grade belongs to a class owned by the authenticated teacher.
        /// </remarks>
        /// <response code="204">If the grade was successfully updated.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="403">If the teacher does not own the class the grade belongs to.</response>
        /// <response code="404">If no teacher profile is found or the grade is not found.</response>
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

        /// <summary>
        /// Deletes an existing grade.
        /// </summary>
        /// <param name="id">ID of the grade to delete.</param>
        /// <returns>No content on successful deletion.</returns>
        /// <remarks>
        /// This endpoint allows teachers to delete an existing grade.
        /// It verifies that the grade belongs to a class owned by the authenticated teacher.
        /// </remarks>
        /// <response code="204">If the grade was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="403">If the teacher does not own the class the grade belongs to.</response>
        /// <response code="404">If no teacher profile is found or the grade is not found.</response>
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

        /// <summary>
        /// Imports multiple grades from a CSV file.
        /// </summary>
        /// <param name="classId">ID of the class to assign grades to.</param>
        /// <param name="file">CSV file containing grade data.</param>
        /// <returns>Status message and count of imported grades.</returns>
        /// <remarks>
        /// This endpoint allows teachers to bulk upload grades from a CSV file.
        /// The CSV file should contain studentId and value columns.
        /// It performs validation on each record, ensuring students exist, are enrolled,
        /// and that grade values are valid (between 1 and 10).
        /// </remarks>
        /// <response code="200">Returns success message with count of imported grades.</response>
        /// <response code="400">If the file is missing/empty or contains validation errors.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no teacher profile is found, or the class is not found, or the teacher does not own the class.</response>
        /// <response code="500">If a server error occurs during file processing or database operations.</response>
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