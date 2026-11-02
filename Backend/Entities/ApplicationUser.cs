using Microsoft.AspNetCore.Identity;
using USMS.API.Entities;

namespace USMS.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
    public Employee? Employee { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
