using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using USMS.Core.DTOs.Complaints;
using USMS.Core.Interfaces;
using USMS.Infrastructure.Persistence;

namespace USMS.API.Controllers
{
    /// <summary>
    /// API controller for managing student complaints, including creation, retrieval, department assignment, and status
    /// updates.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IComplaintService _service;

        public ComplaintsController(ApplicationDbContext context, IComplaintService service)
        {
            _context = context;
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> CreateComplaint(CreateComplaintRequestDto request)
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
                var result = await _service.CreateComplaintAsync(
                    request, student.Id);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("my-complaints")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return Forbid();

            var complaints = await _service.GetMyComplaintsAsync(
                student.Id);

            return Ok(complaints);
        }


        [HttpPut("{id:int}/department")]
        //[Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> AssignComplaintDepartment(int id,
          AssignComplaintDepartmentRequestDto request)
        {
            var result = await _service.AssignComplaintDepartmentAsync(
             id, request.DepartmentId);

            if(!result)
            {
                return BadRequest(new
                {
                    message = "Complaint could not be assigned to the department."
                });
            }

            return Ok(new
            {
                message = "Complaint department assigned successfully."
            });
        }


        [HttpPut("{id:int}/status")]
      //  [Authorize(Roles = "Employee")]

        public async Task<IActionResult> UpdateComplaintStatus(int id,
           UpdateComplaintStatusRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (userId == null)
                return Unauthorized();


            var employee = await _context.Employees
               .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                return Forbid();


            var result = await _service.UpdateComplaintStatusAsync(
               id,
               employee.Id,
               employee.DepartmentId,
               request);

            if (!result)
                return BadRequest(new
                {
                    message = "Complaint status could not be updated."
                });

            return Ok(new
            {
                message = "Complaint status updated successfully."
            });



        }

    }
}
