using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.Core.Entities;

namespace USMS.Infrastructure.Persistence.Configurations;

public class ServiceInformationConfiguration : IEntityTypeConfiguration<ServiceInformation>
{
    public void Configure(EntityTypeBuilder<ServiceInformation> builder)
    {
        builder.Property(si => si.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(si => si.EligibilityConditions)
                .IsRequired();

        builder.Property(si => si.RequiredDocuments)
               .IsRequired();

        builder.Property(si => si.Instructions)
                .IsRequired();

        builder.Property(si => si.ExpectedProcessingDuration)
                .IsRequired();


        builder.HasOne(si => si.Service)
                .WithOne(s => s.ServiceInformation)
                .HasForeignKey<ServiceInformation>(
                    si => si.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

    }
}
