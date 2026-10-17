using Microsoft.AspNetCore.Identity;
using USMS.API.Entities;

namespace USMS.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = "Active"; 
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
