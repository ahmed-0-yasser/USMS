
using System;

namespace USMS.Core.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int AppointmentSlotId { get; set; }

        public int StudentId { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public AppointmentSlot AppointmentSlot { get; set; } = null!;


        public Student Student { get; set; } = null!;
    }
}


