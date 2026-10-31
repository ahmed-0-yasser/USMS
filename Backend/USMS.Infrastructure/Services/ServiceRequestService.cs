using Microsoft.EntityFrameworkCore;
using USMS.Core.DTOs.ServiceRequests;
using USMS.Core.Entities;
using USMS.Infrastructure.Persistence;
using USMS.Core.Interfaces;

namespace USMS.Infrastructure.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequestDetailsDto> CreateRequestAsync(CreateServiceRequestDto dto, int studentId)
        {
            // 1. Validate request type
            string requestType = dto.RequestType.Trim();

            bool isCertificateRequest = requestType.Equals(
                "Certificate Request",
                StringComparison.OrdinalIgnoreCase);

            bool isEnrollmentRequest = requestType.Equals(
                "Enrollment Confirmation Request",
                StringComparison.OrdinalIgnoreCase);

            if (!isCertificateRequest && !isEnrollmentRequest)
            {
                throw new ArgumentException("Invalid request type.");
            }

            // 2. Validate request-specific fields
            if (isCertificateRequest &&
                string.IsNullOrWhiteSpace(dto.CertificateType))
            {
                throw new ArgumentException(
                    "Certificate type is required.");
            }

            if (isEnrollmentRequest &&
                (string.IsNullOrWhiteSpace(dto.AcademicYear) ||
                 string.IsNullOrWhiteSpace(dto.Semester)))
            {
                throw new ArgumentException(
                    "Academic year and semester are required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Purpose))
            {
                throw new ArgumentException("Purpose is required.");
            }

            // 3. Check if student exists
            bool studentExists = await _context.Students
                .AnyAsync(s => s.Id == studentId);

            if (!studentExists)
            {
                throw new ArgumentException("Student not found.");
            }

            // 4. Check if service exists and is active
            var service = await _context.Services
                .FirstOrDefaultAsync(s =>
                    s.Id == dto.ServiceId && s.IsActive);

            if (service == null)
            {
                throw new ArgumentException(
                    "Service not found or inactive.");
            }

            // 5. Create the request
            var request = new ServiceRequest
            {
                RequestType = isCertificateRequest
                    ? "Certificate Request"
                    : "Enrollment Confirmation Request",

                Status = "Submitted",

                CertificateType = isCertificateRequest
                    ? dto.CertificateType!.Trim()
                    : null,

                Purpose = dto.Purpose.Trim(),

                AcademicYear = isEnrollmentRequest
                    ? dto.AcademicYear!.Trim()
                    : null,

                Semester = isEnrollmentRequest
                    ? dto.Semester!.Trim()
                    : null,

                Notes = string.IsNullOrWhiteSpace(dto.Notes)
                    ? null
                    : dto.Notes.Trim(),

                CreatedAt = DateTime.UtcNow,

                StudentId = studentId,

                ServiceId = dto.ServiceId
            };

            // 6. Save to database
            _context.ServiceRequests.Add(request);

            await _context.SaveChangesAsync();

            // 7. Return request details
            return new ServiceRequestDetailsDto
            {
                Id = request.Id,
                RequestType = request.RequestType,
                ServiceName = service.Name,
                Status = request.Status,
                CertificateType = request.CertificateType,
                Purpose = request.Purpose,
                AcademicYear = request.AcademicYear,
                Semester = request.Semester,
                Notes = request.Notes,
                RejectionReason = request.RejectionReason,
                AssignedEmployeeName = null,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt
            };
        }


        public async Task<IEnumerable<ServiceRequestResponseDto>> GetMyRequestsAsync(int studentId)
        {
            var requests = await _context.ServiceRequests
                .Where(sr => sr.StudentId == studentId)
                .OrderByDescending(sr => sr.CreatedAt)
                .Select(sr => new ServiceRequestResponseDto
                {
                    Id = sr.Id,
                    RequestType = sr.RequestType,
                    ServiceName = sr.Service.Name,
                    Status = sr.Status,
                    CreatedAt = sr.CreatedAt,
                    UpdatedAt = sr.UpdatedAt
                })
                .ToListAsync();

            return requests;
        }


        public async Task<ServiceRequestDetailsDto?> GetRequestDetailsAsync(int requestId, int studentId)
        {
            var request = await _context.ServiceRequests
                .Where(sr => sr.Id == requestId && sr.StudentId == studentId)
                .Select(sr => new ServiceRequestDetailsDto
                {
                    Id = sr.Id,
                    RequestType = sr.RequestType,
                    ServiceName = sr.Service.Name,
                    Status = sr.Status,
                    CertificateType = sr.CertificateType,
                    Purpose = sr.Purpose,
                    AcademicYear = sr.AcademicYear,
                    Semester = sr.Semester,
                    Notes = sr.Notes,
                    RejectionReason = sr.RejectionReason,
                    AssignedEmployeeName = sr.AssignedEmployee != null
                        ? sr.AssignedEmployee.FullName
                        : null,
                    CreatedAt = sr.CreatedAt,
                    UpdatedAt = sr.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return request;
        }


        public async Task<bool> CancelRequestAsync(int requestId, int studentId)
        {
            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(sr =>
                    sr.Id == requestId &&
                    sr.StudentId == studentId);

            if (request == null)
            {
                return false;
            }

            if (request.Status != "Submitted")
            {
                return false;
            }

            request.Status = "Cancelled";
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<IEnumerable<ServiceRequestResponseDto>> GetDepartmentRequestsAsync(int departmentId)
        {
            var requests = await _context.ServiceRequests
                .Where(sr => sr.Service.DepartmentId == departmentId)
                .OrderByDescending(sr => sr.CreatedAt)
                .Select(sr => new ServiceRequestResponseDto
                {
                    Id = sr.Id,
                    RequestType = sr.RequestType,
                    ServiceName = sr.Service.Name,
                    Status = sr.Status,
                    CreatedAt = sr.CreatedAt,
                    UpdatedAt = sr.UpdatedAt
                })
                .ToListAsync();

            return requests;
        }


        public async Task<bool> UpdateRequestStatusAsync(int requestId, int employeeId, int departmentId, UpdateServiceRequestStatusDto dto)
        {
            var request = await _context.ServiceRequests
                .Include(sr => sr.Service)
                .FirstOrDefaultAsync(sr =>
                    sr.Id == requestId &&
                    sr.Service.DepartmentId == departmentId);

            if (request == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return false;
            }

            // Validate the new status
            string newStatus = dto.Status.Trim();

            // Check allowed status transitions
            bool isValidTransition =
                (request.Status == "Submitted" && newStatus == "Under Review") ||
                (request.Status == "Under Review" &&
                    (newStatus == "Approved" || newStatus == "Rejected")) ||
                (request.Status == "Approved" && newStatus == "Ready") ||
                (request.Status == "Ready" && newStatus == "Completed");

            if (!isValidTransition)
            {
                return false;
            }

            // Validate rejection reason
            if (newStatus == "Rejected" &&
                string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                return false;
            }

            // Check employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.DepartmentId == departmentId);

            if (employee == null)
            {
                return false;
            }

            // Update request
            request.Status = newStatus;

            request.RejectionReason = newStatus == "Rejected"
                ? dto.RejectionReason!.Trim()
                : null;

            request.AssignedEmployeeId = employeeId;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
