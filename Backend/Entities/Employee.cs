namespace USMS.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}