namespace USMS.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ResponsibleDepartmentId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Note: Uncomment after Student entity is created
        // public Student Student { get; set; } = null!;

        // Note: Uncomment after Department entity is created
        // public Department ResponsibleDepartment { get; set; } = null!;
    }
}
