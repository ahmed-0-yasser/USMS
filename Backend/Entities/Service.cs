namespace USMS.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool SupportsAppointments { get; set; }


        //Note:Uncomment after Department entity is created 
        // public Department Department { get; set; } = null!;
        
         public Department Department { get; set; } = null!;



        // Uncomment after Appointment entity is created
        // public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();



        //Uncomment after ServiceRequest entity is created
        // public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();



        public ServiceInformation? ServiceInformation { get; set; }

    }
}
