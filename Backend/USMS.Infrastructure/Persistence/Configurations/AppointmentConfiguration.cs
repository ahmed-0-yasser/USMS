
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Entities;

namespace USMS.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AppointmentSlotId)
                .IsRequired();

            builder.Property(a => a.StudentId)
                .IsRequired();

            builder.Property(a => a.BookedAt)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.HasOne(a => a.Student)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AppointmentSlot)
                .WithMany(slot => slot.Appointments)
                .HasForeignKey(a => a.AppointmentSlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
