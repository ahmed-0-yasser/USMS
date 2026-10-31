
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using USMS.Core.DTOs.ServiceRequests;
using USMS.Core.Interfaces;
using USMS.Infrastructure.Persistence;

namespace USMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _service;
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(IServiceRequestService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        // 1. Create a new service request
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateServiceRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return Forbid();

            try
            {
                var result = await _service.CreateRequestAsync(
                    dto, student.Id);

                return CreatedAtAction(
                    nameof(GetRequestDetails),
                    new { id = result.Id },
                    result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. Get all requests belonging to the current student
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return Forbid();

            var requests = await _service.GetMyRequestsAsync(student.Id);

            return Ok(requests);
        }

        // 3. Get details of a specific student request
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRequestDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return Forbid();

            var request = await _service.GetRequestDetailsAsync(id, student.Id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        // 4. Cancel a student request
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return Forbid();

            var result = await _service.CancelRequestAsync(id, student.Id);

            if (!result)
                return BadRequest(new
                {
                    message = "Request cannot be cancelled."
                });

            return Ok(new
            {
                message = "Request cancelled successfully."
            });
        }

        // 5. Get requests belonging to the employee's department
        [HttpGet("department")]
        public async Task<IActionResult> GetDepartmentRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                return Forbid();

            var requests = await _service.GetDepartmentRequestsAsync(employee.DepartmentId);

            return Ok(requests);
        }

        // 6. Update the status of a request
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] UpdateServiceRequestStatusDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                return Forbid();

            var result = await _service.UpdateRequestStatusAsync(id, employee.Id, employee.DepartmentId, dto);

            if (!result)
                return BadRequest(new
                {
                    message = "Request status could not be updated. Check the status transition and rejection reason."
                });

            return Ok(new
            {
                message = "Request status updated successfully."
            });
        }
    }
}
