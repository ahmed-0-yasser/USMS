
namespace USMS.Core.DTOs.ServiceRequests
{
    public class ServiceRequestDetailsDto
    {
        public int Id { get; set; }

        public string RequestType { get; set; } = string.Empty;

        public string ServiceName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? CertificateType { get; set; }

        public string Purpose { get; set; } = string.Empty;

        public string? AcademicYear { get; set; }

        public string? Semester { get; set; }

        public string? Notes { get; set; }

        public string? RejectionReason { get; set; }

        public string? AssignedEmployeeName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
