
using Microsoft.EntityFrameworkCore;
using USMS.Core.DTOs.Complaints;
using USMS.Core.Entities;
using USMS.Core.Interfaces;
using USMS.Infrastructure.Persistence;

namespace USMS.Infrastructure.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly ApplicationDbContext _context;

        public ComplaintService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ComplaintResponseDto> CreateComplaintAsync(
            CreateComplaintRequestDto request, int studentId)
        {
            

            if (request.Description.Length > 300)
            {
                throw new ArgumentException(
                    "Description cannot exceed 300 characters.");
            }

            bool studentExists = await _context.Students
                .AnyAsync(s => s.Id == studentId);

            if (!studentExists)
            {
                throw new ArgumentException("Student not found.");
            }

            var complaint = new Complaint
            {
                StudentId = studentId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Category = request.Category.Trim(),
                Status = "Submitted",
                CreatedAt = DateTime.UtcNow
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return new ComplaintResponseDto
            {
                Id = complaint.Id,
                Title = complaint.Title,
                Description = complaint.Description,
                Category = complaint.Category,
                Status = complaint.Status,
                ResponsibleDepartmentId = complaint.ResponsibleDepartmentId,
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt
            };
        }

        public async Task<List<ComplaintResponseDto>> GetMyComplaintsAsync(
            int studentId)
        {
            var complaints = await _context.Complaints
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Category = c.Category,
                    Status = c.Status,
                    ResponsibleDepartmentId = c.ResponsibleDepartmentId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return complaints;
        }

        public async Task<bool> AssignComplaintDepartmentAsync(
            int complaintId, int departmentId)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == complaintId);

            if (complaint == null)
                return false;

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == departmentId);

            if (!departmentExists)
                return false;


            var studentUserId = await _context.Students
              .Where(s => s.Id == complaint.StudentId)
              .Select(s => s.UserId)
              .FirstOrDefaultAsync();

            if (studentUserId == null)
                return false;

            complaint.ResponsibleDepartmentId = departmentId;
            complaint.UpdatedAt = DateTime.UtcNow;


            _context.Notifications.Add(new Notification
            {
                UserId = studentUserId,
                Title = "Complaint Updated",
                Message = $"A department has been assigned to your complaint: {complaint.Title}",
                Type = "Complaint",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateComplaintStatusAsync(
            int complaintId,
            int employeeId,
            int departmentId,
            UpdateComplaintStatusRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
                return false;

            string newStatus = request.Status.Trim().ToLowerInvariant();

            string status;

            if (newStatus == "submitted")
                status = "Submitted";
            else if (newStatus == "under investigation")
                status = "Under Investigation";
            else if (newStatus == "resolved")
                status = "Resolved";
            else
                return false;

            var employeeExists = await _context.Employees
                .AnyAsync(e =>
                    e.Id == employeeId &&
                    e.DepartmentId == departmentId);

            if (!employeeExists)
                return false;

            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c =>
                    c.Id == complaintId &&
                    c.ResponsibleDepartmentId == departmentId);

            if (complaint == null)
                return false;


            var studentUserId = await _context.Students
             .Where(s => s.Id == complaint.StudentId)
             .Select(s => s.UserId)
             .FirstOrDefaultAsync();



            if (studentUserId == null)
                return false;

            complaint.Status = status;
            complaint.UpdatedAt = DateTime.UtcNow;


            _context.Notifications.Add(new Notification
            {
                UserId = studentUserId,
                Title = "Complaint Status Updated",
                Message = $"Your complaint '{complaint.Title}' status is now: {status}.",
                Type = "Complaint",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });


            await _context.SaveChangesAsync();

            return true;
        }
    }
}
