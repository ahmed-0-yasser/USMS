using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using USMS.API.Configurations;
using USMS.API.Entities;
using USMS.Entities;
using USMS.Persistence.Configurations;

namespace USMS.API.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new NotificationConfiguration());
            builder.ApplyConfiguration(new ServiceRequestConfiguration());
        }
    }
}