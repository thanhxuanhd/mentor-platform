using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserTeachingApproachConfiguration : IEntityTypeConfiguration<UserTeachingApproach>
{
    public void Configure(EntityTypeBuilder<UserTeachingApproach> builder)
    {
        builder.HasKey(ue => ue.Id);

        builder.HasIndex(ue => new { ue.UserId, ue.TeachingApproachId });

        builder.HasOne(ue => ue.User)
            .WithMany(u => u.UserTeachingApproaches)
            .HasForeignKey(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.TeachingApproach)
            .WithMany(u => u.UserTeachingApproaches)
            .HasForeignKey(ue => ue.TeachingApproachId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}