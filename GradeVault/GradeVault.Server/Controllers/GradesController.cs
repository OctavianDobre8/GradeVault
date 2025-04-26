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
    public class GradesController : ApiControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly ILogger _logger;

        public GradesController(AppDatabaseContext context)
        {
            _context = context;
            _logger = Log.ForContext<GradesController>();
        }

        // GET: api/grades/assignment/{id}
        [HttpGet("assignment/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetGradesByAssignment(int id)
        {
            try
            {
                var grades = await _context.Grades
                    .Where(g => g.ClassId == id)
                    .Select(g => new
                    {
                        id = g.Id,
                        studentId = g.StudentId,
                        score = g.Value,
                        dateAssigned = g.DateAssigned
                    })
                    .ToListAsync();

                return Ok(grades);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving grades for assignment {AssignmentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/grades
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateGrade([FromBody] GradeCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verify the student exists
                var student = await _context.Students.FindAsync(model.StudentId);
                if (student == null)
                {
                    return NotFound($"Student with ID {model.StudentId} not found");
                }

                // Create new grade
                var grade = new Grade
                {
                    StudentId = model.StudentId,
                    ClassId = model.AssignmentId, // Using ClassId to store AssignmentId for now
                    Value = model.Score,
                    DateAssigned = DateTime.UtcNow
                };

                _context.Grades.Add(grade);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, new
                {
                    id = grade.Id,
                    studentId = grade.StudentId,
                    score = grade.Value,
                    dateAssigned = grade.DateAssigned
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating grade");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/grades/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> GetGrade(int id)
        {
            try
            {
                var grade = await _context.Grades.FindAsync(id);

                if (grade == null)
                {
                    return NotFound($"Grade with ID {id} not found");
                }

                return Ok(new
                {
                    id = grade.Id,
                    studentId = grade.StudentId,
                    score = grade.Value,
                    dateAssigned = grade.DateAssigned
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving grade with ID {GradeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/grades/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] GradeUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grade = await _context.Grades.FindAsync(id);
                if (grade == null)
                {
                    return NotFound($"Grade with ID {id} not found");
                }

                // Update grade
                grade.Value = model.Score;
                
                _context.Grades.Update(grade);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = grade.Id,
                    studentId = grade.StudentId,
                    score = grade.Value,
                    dateAssigned = grade.DateAssigned
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating grade with ID {GradeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/grades/student/{id}
        [HttpGet("student/{id}")]
        [Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> GetGradesByStudent(int id)
        {
            try
            {
                var grades = await _context.Grades
                    .Where(g => g.StudentId == id)
                    .Select(g => new
                    {
                        id = g.Id,
                        classId = g.ClassId,
                        score = g.Value,
                        dateAssigned = g.DateAssigned
                    })
                    .ToListAsync();

                return Ok(grades);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving grades for student {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class GradeCreateDTO
    {
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public int Score { get; set; }
    }

    public class GradeUpdateDTO
    {
        public int Score { get; set; }
    }
}