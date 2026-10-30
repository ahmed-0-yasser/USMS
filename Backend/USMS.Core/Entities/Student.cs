using System.Collections.Generic;
using USMS.Core.Entities;

namespace USMS.Core.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string StudentNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Navigation Property: A student can submit many service requests
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        // Navigation Property: Link Student to ApplicationUser
        public ApplicationUser User { get; set; } = null!;
    }
}
