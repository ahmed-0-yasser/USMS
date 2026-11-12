
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using USMS.Core.DTOs.ServiceRequests;
using USMS.Core.Interfaces;
using USMS.Infrastructure.Persistence;

namespace USMS.API.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to student service requests,
    /// including creation, retrieval, cancellation, and status updates.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _service;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the ServiceRequestsController class.
        /// </summary>
        /// <param name="service">The service responsible for handling service request business logic.</param>
        /// <param name="context">The application database context used to retrieve student and employee information.</param>
        public ServiceRequestsController(IServiceRequestService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        // 1. Create a new service request

        /// <summary>
        /// Creates a new service request for the currently authenticated student.
        /// </summary>
        /// <param name="dto">The data required to create the service request.</param>
        /// <returns>
        /// 201 Created with the created request details;
        /// 400 Bad Request if the request data is invalid;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the authenticated user is not a registered student.
        /// </returns>
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

        /// <summary>
        /// Retrieves all service requests belonging to the currently authenticated student.
        /// </summary>
        /// <returns>
        /// 200 OK with the student's service requests;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the authenticated user is not a registered student.
        /// </returns>
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

        /// <summary>
        /// Retrieves the details of a specific service request belonging to the currently authenticated student.
        /// </summary>
        /// <param name="id">The ID of the service request.</param>
        /// <returns>
        /// 200 OK with the request details;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the authenticated user is not a registered student;
        /// 404 Not Found if the request does not exist or does not belong to the student.
        /// </returns>
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

        /// <summary>
        /// Cancels a service request belonging to the currently authenticated student,
        /// provided that the request satisfies the cancellation rules.
        /// </summary>
        /// <param name="id">The ID of the service request to cancel.</param>
        /// <returns>
        /// 200 OK if the request is successfully cancelled;
        /// 400 Bad Request if the request cannot be cancelled;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the authenticated user is not a registered student.
        /// </returns>
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

        /// <summary>
        /// Retrieves service requests associated with the department
        /// of the currently authenticated employee.
        /// </summary>
        /// <returns>
        /// 200 OK with the department's service requests;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the authenticated user is not a registered employee.
        /// </returns>
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

        /// <summary>
        /// Updates the status of a service request within the authenticated employee's department.
        /// </summary>
        /// <param name="id">The ID of the service request to update.</param>
        /// <param name="dto">The new status and any required rejection reason.</param>
        /// <returns>
        /// 200 OK if the request status is successfully updated;
        /// 400 Bad Request if the status transition or rejection reason is invalid;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the authenticated user is not a registered employee.
        /// </returns>
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
