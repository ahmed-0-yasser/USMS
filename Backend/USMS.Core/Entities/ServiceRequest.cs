
namespace USMS.Core.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        public string RequestType { get; set; } = string.Empty;

        public string Status { get; set; } = "Submitted";

        public string? RejectionReason { get; set; }

        public string? CertificateType { get; set; }

        public string Purpose { get; set; } = string.Empty;

        public string? AcademicYear { get; set; }

        public string? Semester { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }

        public int ServiceId { get; set; }

        public int? AssignedEmployeeId { get; set; }

        // Navigation Properties
        public Student Student { get; set; } = null!;

        public Service Service { get; set; } = null!;

        public Employee? AssignedEmployee { get; set; }
    }
}


