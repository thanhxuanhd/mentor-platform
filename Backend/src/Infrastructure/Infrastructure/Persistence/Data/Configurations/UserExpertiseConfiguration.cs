using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserExpertiseConfiguration : IEntityTypeConfiguration<UserExpertise>
{
    public void Configure(EntityTypeBuilder<UserExpertise> builder)
    {
        builder.HasKey(ue => ue.Id);

        builder.HasIndex(ue => new { ue.UserId, ue.ExpertiseId })
            .IsUnique();

        builder.HasOne(ue => ue.User)
            .WithMany(u => u.UserExpertises)
            .HasForeignKey(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Expertise)
            .WithMany(u => u.UserExpertises)
            .HasForeignKey(ue => ue.ExpertiseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}