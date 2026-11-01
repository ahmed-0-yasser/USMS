
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Entities;

namespace USMS.Persistence.Configurations
{
    public class AppointmentSlotConfiguration : IEntityTypeConfiguration<AppointmentSlot>
    {
        public void Configure(EntityTypeBuilder<AppointmentSlot> builder)
        {
            builder.Property(a => a.StartTime)
                .IsRequired();

            builder.Property(a => a.EndTime)
                .IsRequired();

            builder.Property(a => a.Capacity)
                .IsRequired()
                .HasDefaultValue(10);


            builder.HasOne(a => a.Service)
                .WithMany(s => s.AppointmentSlots)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(a => a.Appointments)
                .WithOne(a => a.AppointmentSlot)
                .HasForeignKey(a => a.AppointmentSlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
