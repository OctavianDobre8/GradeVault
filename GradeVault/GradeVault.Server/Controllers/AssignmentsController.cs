using GradeVault.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GradeVault.Server.Controllers
{
    [Route("api/[controller]")]
    public class AssignmentsController : ApiControllerBase
    {
        private readonly AppDatabaseContext _context;
        private readonly ILogger _logger;

        public AssignmentsController(AppDatabaseContext context)
        {
            _context = context;
            _logger = Log.ForContext<AssignmentsController>();
        }

        // GET: api/assignments/class/{id}
        [HttpGet("class/{id}")]
        [Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> GetAssignmentsByClass(int id)
        {
            try
            {
                // For now, return the class as the assignment to simplify
                var classData = await _context.Classes.FindAsync(id);
                
                if (classData == null)
                {
                    return NotFound($"Class with ID {id} not found");
                }

                var assignment = new
                {
                    id = classData.Id,
                    title = classData.Name,
                    type = "Class Assignment",
                    date = DateTime.Now,
                    maxPoints = 100,
                    description = classData.Description ?? "No description available"
                };
                
                return Ok(new[] { assignment });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving assignments for class {ClassId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/assignments/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> GetAssignment(int id)
        {
            try
            {
                var classData = await _context.Classes.FindAsync(id);
                
                if (classData == null)
                {
                    return NotFound($"Assignment with ID {id} not found");
                }

                return Ok(new
                {
                    id = classData.Id,
                    title = classData.Name,
                    type = "Class Assignment",
                    date = DateTime.Now,
                    maxPoints = 100,
                    description = classData.Description ?? "No description available"
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving assignment with ID {AssignmentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}