
using Microsoft.EntityFrameworkCore;
using USMS.Core.DTOs.ServiceRequests;
using USMS.Core.Entities;
using USMS.Infrastructure.Persistence;
using USMS.Core.Interfaces;

namespace USMS.Infrastructure.Services
{
    /// <summary>
    /// Provides the business logic for managing student service requests,
    /// including creation, retrieval, cancellation, and status updates.
    /// </summary>
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the ServiceRequestService class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ServiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new service request after validating the request type,
        /// required fields, student, and selected service.
        /// </summary>
        /// <param name="dto">The data required to create the service request.</param>
        /// <param name="studentId">The ID of the student submitting the request.</param>
        /// <returns>The details of the newly created service request.</returns>
        public async Task<ServiceRequestDetailsDto> CreateRequestAsync(
            CreateServiceRequestDto dto,
            int studentId)
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

        /// <summary>
        /// Retrieves all service requests submitted by a specific student,
        /// ordered from newest to oldest.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        /// <returns>A collection of the student's service request summaries.</returns>
        public async Task<IEnumerable<ServiceRequestResponseDto>>
            GetMyRequestsAsync(int studentId)
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

        /// <summary>
        /// Retrieves the details of a specific service request
        /// only if it belongs to the specified student.
        /// </summary>
        /// <param name="requestId">The ID of the service request.</param>
        /// <param name="studentId">The ID of the student requesting the details.</param>
        /// <returns>
        /// The service request details if found; otherwise, null.
        /// </returns>
        public async Task<ServiceRequestDetailsDto?>
            GetRequestDetailsAsync(int requestId, int studentId)
        {
            var request = await _context.ServiceRequests
                .Where(sr =>
                    sr.Id == requestId &&
                    sr.StudentId == studentId)
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

        /// <summary>
        /// Cancels a service request if it belongs to the specified student
        /// and its current status is Submitted.
        /// </summary>
        /// <param name="requestId">The ID of the service request to cancel.</param>
        /// <param name="studentId">The ID of the student requesting cancellation.</param>
        /// <returns>
        /// True if the request was successfully cancelled; otherwise, false.
        /// </returns>
        public async Task<bool> CancelRequestAsync(
            int requestId, int studentId)
        {
            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(sr =>
                    sr.Id == requestId &&
                    sr.StudentId == studentId);

            if (request == null)
            {
                return false;
            }

            if (!string.Equals(
                request.Status,
                "Submitted",
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            request.Status = "Cancelled";
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Retrieves all service requests associated with services
        /// belonging to a specific department.
        /// </summary>
        /// <param name="departmentId">The ID of the department.</param>
        /// <returns>A collection of service request summaries for the department.</returns>
        public async Task<IEnumerable<ServiceRequestResponseDto>>
            GetDepartmentRequestsAsync(int departmentId)
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

        /// <summary>
        /// Updates a service request's status after validating the allowed
        /// status transition, rejection reason, and employee department.
        /// </summary>
        /// <param name="requestId">The ID of the service request.</param>
        /// <param name="employeeId">The ID of the employee updating the request.</param>
        /// <param name="departmentId">The ID of the employee's department.</param>
        /// <param name="dto">The new status and optional rejection reason.</param>
        /// <returns>
        /// True if the status was successfully updated; otherwise, false.
        /// </returns>
        public async Task<bool> UpdateRequestStatusAsync(
            int requestId,
            int employeeId,
            int departmentId,
            UpdateServiceRequestStatusDto dto)
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

            // 1. Validate and normalize the new status
            string newStatus = dto.Status.Trim().ToLowerInvariant() switch
            {
                "under review" => "Under Review",
                "approved" => "Approved",
                "rejected" => "Rejected",
                "ready" => "Ready",
                "completed" => "Completed",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(newStatus))
            {
                return false;
            }

            // 2. Check allowed status transitions
            bool isValidTransition =
                (string.Equals(
                    request.Status,
                    "Submitted",
                    StringComparison.OrdinalIgnoreCase) &&
                 newStatus == "Under Review") ||

                (string.Equals(
                    request.Status,
                    "Under Review",
                    StringComparison.OrdinalIgnoreCase) &&
                 (newStatus == "Approved" || newStatus == "Rejected")) ||

                (string.Equals(
                    request.Status,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase) &&
                 newStatus == "Ready") ||

                (string.Equals(
                    request.Status,
                    "Ready",
                    StringComparison.OrdinalIgnoreCase) &&
                 newStatus == "Completed");

            if (!isValidTransition)
            {
                return false;
            }

            // 3. Validate rejection reason
            if (newStatus == "Rejected" &&
                string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                return false;
            }

            // 4. Check employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.DepartmentId == departmentId);

            if (employee == null)
            {
                return false;
            }

            // 5. Update request
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
