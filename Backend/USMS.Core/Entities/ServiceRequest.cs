
namespace USMS.Core.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        public string RequestType { get; set; } = string.Empty;

        public string Status { get; set; } = "Submitted";

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }

        public int ServiceId { get; set; }

        public string? AssignedEmployeeId { get; set; }

        // Navigation Properties
        public Student Student { get; set; } = null!;

        public Service Service { get; set; } = null!;

        public ApplicationUser? AssignedEmployee { get; set; }
    }
}


