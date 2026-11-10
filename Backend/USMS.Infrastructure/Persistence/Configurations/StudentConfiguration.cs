
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Entities;

namespace USMS.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.StudentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.HasIndex(s => s.StudentNumber)
                .IsUnique();

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
