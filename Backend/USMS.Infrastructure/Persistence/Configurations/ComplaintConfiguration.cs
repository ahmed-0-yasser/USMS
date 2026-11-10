
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Core.Entities;

namespace USMS.Persistence.Configurations
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {

            builder.Property(c => c.Title)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(c => c.Category)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasDefaultValue("Pending");


            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasOne(c => c.Student)
                .WithMany(s => s.Complaints)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(c => c.ResponsibleDepartment)
                .WithMany()
                .HasForeignKey(c => c.ResponsibleDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
