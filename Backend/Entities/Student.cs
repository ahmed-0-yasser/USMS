
using System.Collections.Generic;

namespace USMS.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string StudentNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Note: Uncomment after ServiceRequest entity is created
        // public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        // Note: Uncomment after Complaint entity is created
        // public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        // Note: Uncomment after ApplicationUser relationship is configured
        // public ApplicationUser User { get; set; } = null!;
    }
}
