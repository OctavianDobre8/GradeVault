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
    [Route("api/classes")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public ClassController(AppDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

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