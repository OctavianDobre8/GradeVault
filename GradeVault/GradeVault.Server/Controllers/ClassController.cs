using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GradeVault.Server.Models;
using GradeVault.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradeVault.Server.Controllers
{
    /// <summary>
    /// Controller for managing class-related operations in the GradeVault system.
    /// </summary>
    /// <remarks>
    /// Handles class creation, updating, deletion, and student enrollment management.
    /// Provides endpoints for teachers to manage their classes and associated students.
    /// </remarks>
    [Route("api/classes")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the ClassController.
        /// </summary>
        /// <param name="context">The database context for data operations.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        public ClassController(AppDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves all classes for the authenticated teacher.
        /// </summary>
        /// <returns>Collection of classes taught by the current teacher.</returns>
        /// <remarks>
        /// This endpoint fetches classes where the current user is assigned as the teacher,
        /// returning class details along with the teacher's name.
        /// </remarks>
        /// <response code="200">Returns the list of classes taught by the teacher.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no teacher profile is found for the authenticated user.</response>
        [HttpGet("teacher-classes")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<ClassDTO>>> GetTeacherClasses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            var classes = await _context.Classes
                .Where(c => c.TeacherId == teacher.Id)
                .Select(c => new ClassDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    RoomNumber = c.RoomNumber,
                    TeacherName = $"{teacher.FirstName} {teacher.LastName}"
                })
                .ToListAsync();

            return classes;
        }

        /// <summary>
        /// Gets all students enrolled in a specific class.
        /// </summary>
        /// <param name="classId">ID of the class to get students for.</param>
        /// <returns>Collection of students enrolled in the specified class.</returns>
        /// <remarks>
        /// This endpoint verifies that the class belongs to the requesting teacher before
        /// returning the list of enrolled students.
        /// </remarks>
        /// <response code="200">Returns the list of students enrolled in the class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If the teacher profile or class is not found, or the teacher does not own the class.</response>
        [HttpGet("{classId}/students")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsByClass(int classId)
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
                .AnyAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (!classExists)
            {
                return NotFound("Class not found or you don't have permission to access it.");
            }

            var students = await _context.ClassEnrollments
                .Where(e => e.ClassId == classId)
                .Include(e => e.Student)
                .Select(e => new StudentDTO
                {
                    Id = e.Student.Id,
                    FirstName = e.Student.FirstName,
                    LastName = e.Student.LastName,
                    Email = e.Student.Email
                })
                .ToListAsync();

            return students;
        }

        /// <summary>
        /// Retrieves students who are not yet enrolled in a specific class.
        /// </summary>
        /// <param name="classId">ID of the class to get available students for.</param>
        /// <returns>Collection of students not enrolled in the specified class.</returns>
        /// <remarks>
        /// This endpoint helps teachers find students who could be added to their class by
        /// filtering out students who are already enrolled.
        /// </remarks>
        /// <response code="200">Returns the list of students not enrolled in the class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If the teacher profile or class is not found, or the teacher does not own the class.</response>
        [HttpGet("{classId}/available-students")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAvailableStudents(int classId)
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
                .AnyAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (!classExists)
            {
                return NotFound("Class not found or you don't have permission to access it.");
            }

            // Get students who are not enrolled in this class
            var enrolledStudentIds = await _context.ClassEnrollments
                .Where(e => e.ClassId == classId)
                .Select(e => e.StudentId)
                .ToListAsync();

            var availableStudents = await _context.Students
                .Where(s => !enrolledStudentIds.Contains(s.Id))
                .Select(s => new StudentDTO
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email
                })
                .ToListAsync();

            return availableStudents;
        }

        /// <summary>
        /// Creates a new class for the authenticated teacher.
        /// </summary>
        /// <param name="model">Class creation data transfer object.</param>
        /// <returns>Details of the newly created class.</returns>
        /// <remarks>
        /// This endpoint creates a new class record in the database, associating it with the
        /// authenticated teacher's profile.
        /// </remarks>
        /// <response code="200">Returns the newly created class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If no teacher profile is found for the authenticated user.</response>
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ClassDTO>> CreateClass(CreateClassDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            var newClass = new Class
            {
                Name = model.Name,
                Description = model.Description,
                RoomNumber = model.RoomNumber,
                TeacherId = teacher.Id
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return new ClassDTO
            {
                Id = newClass.Id,
                Name = newClass.Name,
                Description = newClass.Description,
                RoomNumber = newClass.RoomNumber,
                TeacherName = $"{teacher.FirstName} {teacher.LastName}"
            };
        }

        /// <summary>
        /// Updates an existing class's information.
        /// </summary>
        /// <param name="id">ID of the class to update.</param>
        /// <param name="model">Updated class data.</param>
        /// <returns>No content on success.</returns>
        /// <remarks>
        /// This endpoint allows teachers to modify the details of a class they own.
        /// Only name, description, and room number can be updated.
        /// </remarks>
        /// <response code="204">If the class was successfully updated.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If the teacher profile or class is not found, or the teacher does not own the class.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateClass(int id, CreateClassDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            var classToUpdate = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == teacher.Id);

            if (classToUpdate == null)
            {
                return NotFound("Class not found or you don't have permission to modify it.");
            }

            classToUpdate.Name = model.Name;
            classToUpdate.Description = model.Description;
            classToUpdate.RoomNumber = model.RoomNumber;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a class and all associated enrollments and grades.
        /// </summary>
        /// <param name="id">ID of the class to delete.</param>
        /// <returns>No content on success.</returns>
        /// <remarks>
        /// This endpoint performs a cascading delete operation:
        /// 1. Deletes all student enrollments for the class
        /// 2. Deletes all grades associated with the class
        /// 3. Deletes the class itself
        /// </remarks>
        /// <response code="204">If the class was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If the teacher profile or class is not found, or the teacher does not own the class.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                return NotFound("No teacher profile found for this user.");
            }

            var classToDelete = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == teacher.Id);

            if (classToDelete == null)
            {
                return NotFound("Class not found or you don't have permission to delete it.");
            }

            // First check if there are any enrollments for this class
            var hasEnrollments = await _context.ClassEnrollments
                .AnyAsync(e => e.ClassId == id);
                
            if (hasEnrollments)
            {
                // Delete all enrollments for this class
                var enrollments = await _context.ClassEnrollments
                    .Where(e => e.ClassId == id)
                    .ToListAsync();
                    
                _context.ClassEnrollments.RemoveRange(enrollments);
            }
            
            // Then check if there are any grades for this class
            var hasGrades = await _context.Grades
                .AnyAsync(g => g.ClassId == id);
                
            if (hasGrades)
            {
                // Delete all grades for this class
                var grades = await _context.Grades
                    .Where(g => g.ClassId == id)
                    .ToListAsync();
                    
                _context.Grades.RemoveRange(grades);
            }

            _context.Classes.Remove(classToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="classId">ID of the class.</param>
        /// <param name="studentId">ID of the student to enroll.</param>
        /// <returns>No content on success.</returns>
        /// <remarks>
        /// This endpoint creates a new enrollment record linking a student to a class,
        /// allowing the student to receive grades in that class.
        /// </remarks>
        /// <response code="204">If the student was successfully enrolled.</response>
        /// <response code="400">If the student is already enrolled in the class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If the teacher profile, class, or student is not found, or if the teacher does not own the class.</response>
        [HttpPost("{classId}/students/{studentId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddStudentToClass(int classId, int studentId)
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
                .AnyAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (!classExists)
            {
                return NotFound("Class not found or you don't have permission to modify it.");
            }

            // Check if student exists
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == studentId);

            if (!studentExists)
            {
                return NotFound("Student not found.");
            }

            // Check if enrollment already exists
            var enrollmentExists = await _context.ClassEnrollments
                .AnyAsync(e => e.ClassId == classId && e.StudentId == studentId);

            if (enrollmentExists)
            {
                return BadRequest("Student is already enrolled in this class.");
            }

            var enrollment = new ClassEnrollment
            {
                ClassId = classId,
                StudentId = studentId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.ClassEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Removes a student from a class.
        /// </summary>
        /// <param name="classId">ID of the class.</param>
        /// <param name="studentId">ID of the student to remove.</param>
        /// <returns>No content on success.</returns>
        /// <remarks>
        /// This endpoint deletes the enrollment record linking a student to a class.
        /// It also removes any grades the student has received in that class.
        /// </remarks>
        /// <response code="204">If the student was successfully removed from the class.</response>
        /// <response code="401">If the user is not authenticated or not authorized.</response>
        /// <response code="404">If the teacher profile, enrollment record, or class is not found, or if the teacher does not own the class.</response>
        [HttpDelete("{classId}/students/{studentId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> RemoveStudentFromClass(int classId, int studentId)
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
                .AnyAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (!classExists)
            {
                return NotFound("Class not found or you don't have permission to modify it.");
            }

            var enrollment = await _context.ClassEnrollments
                .FirstOrDefaultAsync(e => e.ClassId == classId && e.StudentId == studentId);

            if (enrollment == null)
            {
                return NotFound("Student is not enrolled in this class.");
            }
            
            // Remove any grades for this student in this class
            var grades = await _context.Grades
                .Where(g => g.ClassId == classId && g.StudentId == studentId)
                .ToListAsync();
                
            if (grades.Any())
            {
                _context.Grades.RemoveRange(grades);
            }

            _context.ClassEnrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}