using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Entities;

namespace USMS.API.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(200);

        builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

        builder.Property(s => s.SupportsAppointments)
            .IsRequired();


        builder.HasOne(s => s.Department)
            .WithMany(s=>s.Services)
            .HasForeignKey(s=>s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(s => s.ServiceInformation)
          .WithOne(si => si.Service)
          .HasForeignKey<ServiceInformation>(si => si.ServiceId)
          .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s=>s.AppointmentSlots)
            .WithOne(slot=>slot.Service)
            .HasForeignKey(slot=>slot.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(s => s.ServiceRequests)
            .WithOne(s => s.Service)
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);



    }
}
