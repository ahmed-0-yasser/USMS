using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Core.Entities;

namespace USMS.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Description).HasMaxLength(500);
            builder.Property(d => d.IsActive).IsRequired();

            builder.HasIndex(d => d.Name).IsUnique();

            builder.HasMany(d => d.Employees)
                   .WithOne(e => e.Department)
                   .HasForeignKey(e => e.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Services)
                   .WithOne(s => s.Department)
                   .HasForeignKey(s => s.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
