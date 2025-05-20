using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserAvailabilityConfiguration : IEntityTypeConfiguration<UserAvailability>
{
    public void Configure(EntityTypeBuilder<UserAvailability> builder)
    {
        builder.HasKey(ue => ue.Id);

        builder.HasIndex(ue => new { ue.UserId, ue.AvailabilityId });

        builder.HasOne(ue => ue.User)
            .WithMany(u => u.UserAvailabilities)
            .HasForeignKey(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Availability)
            .WithMany(u => u.UserAvailabilities)
            .HasForeignKey(ue => ue.AvailabilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}