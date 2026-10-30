namespace USMS.Core.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool SupportsAppointments { get; set; }


        
         public Department Department { get; set; } = null!;



        public ICollection<AppointmentSlot> AppointmentSlots { get; set; } = new List<AppointmentSlot>();



        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();



        public ServiceInformation? ServiceInformation { get; set; }

    }
}
