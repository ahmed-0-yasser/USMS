
namespace USMS.Entities
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
        public string StudentId { get; set; } = string.Empty;

        public int ServiceId { get; set; }

        public string? AssignedEmployeeId { get; set; }

        // Navigation Properties
        public ApplicationUser Student { get; set; } = null!;

        public Service Service { get; set; } = null!;

        public ApplicationUser? AssignedEmployee { get; set; }


        /*
         * TODO: Review the relationships after the Student and Employee entities are finalized.
         *
         * - StudentId currently references ApplicationUser.Id.
         * - AssignedEmployeeId currently references ApplicationUser.Id.
         * - Student and AssignedEmployee are navigation properties to ApplicationUser.
         * - Confirm whether Employee should be referenced through its own entity
         *   or directly through ApplicationUser, according to the final ERD.
         * - Configure the foreign keys and relationships in ServiceRequestConfiguration.cs.
         *
         * Note: Do not create a migration until the team finalizes all entities
         * and integrates their configurations into ApplicationDbContext.
         */

    }
}
