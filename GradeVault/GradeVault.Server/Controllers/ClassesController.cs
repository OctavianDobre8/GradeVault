using GradeVault.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Serilog;
using ILogger = Serilog.ILogger; 

namespace GradeVault.Server.Controllers
{
    [Route("api/[controller]")]
    public class ClassesController : ApiControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly ILogger _logger;

        public ClassesController(AppDatabaseContext context)
        {
            _context = context;
            _logger = Log.ForContext<ClassesController>();
        }

        // GET: api/classes/teacher
        [HttpGet("teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherClasses()
        {
            try
            {
                // Get current user ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.Warning("No user ID found in claims for teacher classes request");
                    return Unauthorized("User not authenticated");
                }

                // Find the teacher associated with this user
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.UserId == userId);
                    
                if (teacher == null)
                {
                    _logger.Warning("No teacher profile found for user ID {UserId}", userId);
                    return NotFound("Teacher profile not found");
                }

                // Get classes where this teacher is assigned
                var classes = await _context.Classes
                    .Where(c => c.TeacherId == teacher.Id)
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        subject = c.Subject,
                        description = c.Description,
                        studentCount = c.Enrollments.Count
                    })
                    .ToListAsync();

                _logger.Information("Retrieved {Count} classes for teacher ID {TeacherId}", 
                    classes.Count, teacher.Id);
                return Ok(classes);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving teacher classes");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/classes
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateClass([FromBody] ClassCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.UserId == userId);
                    
                if (teacher == null)
                {
                    return NotFound("Teacher profile not found");
                }

                var newClass = new Class
                {
                    Name = model.Name,
                    Subject = model.Subject,
                    Description = model.Description,
                    TeacherId = teacher.Id
                };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                _logger.Information("Class {ClassName} created by teacher {TeacherId}", 
                    model.Name, teacher.Id);
                
                return CreatedAtAction(nameof(GetClass), new { id = newClass.Id }, new
                {
                    id = newClass.Id,
                    name = newClass.Name,
                    subject = newClass.Subject,
                    description = newClass.Description
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating class");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/classes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClass(int id)
        {
            try
            {
                var classEntity = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (classEntity == null)
                {
                    return NotFound($"Class with ID {id} not found");
                }

                return Ok(new
                {
                    id = classEntity.Id,
                    name = classEntity.Name,
                    subject = classEntity.Subject,
                    description = classEntity.Description
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving class with ID {ClassId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class ClassCreateDTO
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}