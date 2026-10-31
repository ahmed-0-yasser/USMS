
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
            // Primary Key
            builder.HasKey(sr => sr.Id);

            // Properties
            builder.Property(sr => sr.RequestType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sr => sr.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Submitted");

            builder.Property(sr => sr.RejectionReason)
                .HasMaxLength(500);

            builder.Property(sr => sr.CertificateType)
                .HasMaxLength(100);

            builder.Property(sr => sr.Purpose)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(sr => sr.AcademicYear)
                .HasMaxLength(20);

            builder.Property(sr => sr.Semester)
                .HasMaxLength(50);

            builder.Property(sr => sr.Notes)
                .HasMaxLength(500);

            builder.Property(sr => sr.CreatedAt)
                .IsRequired();

            builder.Property(sr => sr.StudentId)
                .IsRequired();

            builder.Property(sr => sr.ServiceId)
                .IsRequired();

            // Student: One Student can have many Service Requests
            builder.HasOne(sr => sr.Student)
                .WithMany(s => s.ServiceRequests)
                .HasForeignKey(sr => sr.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service: One Service can receive many Service Requests
            builder.HasOne(sr => sr.Service)
                .WithMany(s => s.ServiceRequests)
                .HasForeignKey(sr => sr.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee: One Employee can handle many Service Requests
            builder.HasOne(sr => sr.AssignedEmployee)
                .WithMany()
                .HasForeignKey(sr => sr.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
