
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Core.Entities;

namespace USMS.Infrastructure.Persistence.Configurations
{
    public class ServiceRequestConfiguration
        : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.RequestType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sr => sr.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Submitted");

            builder.Property(sr => sr.RejectionReason)
                .HasMaxLength(500);

            builder.Property(sr => sr.CreatedAt)
                .IsRequired();

            builder.Property(sr => sr.StudentId)
                .IsRequired();

            builder.Property(sr => sr.ServiceId)
                .IsRequired();

            // TODO: Configure the relationships after reviewing
            // the final Student and Employee entity designs.
        }
    }
}

