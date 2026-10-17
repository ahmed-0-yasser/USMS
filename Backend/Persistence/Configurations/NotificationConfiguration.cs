using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using USMS.API.Entities;

namespace USMS.API.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired();

        builder.Property(x => x.Message)
               .IsRequired();

        builder.Property(x => x.Type)
               .IsRequired()
               .HasDefaultValue("General");

        builder.Property(x => x.IsRead)
               .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("GETDATE()");

        builder.HasOne(n => n.User)
               .WithMany(u => u.Notifications)
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}