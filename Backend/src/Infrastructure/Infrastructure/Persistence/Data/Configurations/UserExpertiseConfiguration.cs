using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserExpertiseConfiguration : IEntityTypeConfiguration<UserExpertise>
{
    public void Configure(EntityTypeBuilder<UserExpertise> builder)
    {
        builder.HasKey(ue => ue.Id);

        builder.HasIndex(ue => new { ue.UserDetailId, ue.ExpertiseId });

        builder.HasOne(ue => ue.UserDetail)
            .WithMany(u => u.UserExpertises)
            .HasForeignKey(ue => ue.UserDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Expertise)
            .WithMany(u => u.UserExpertises)
            .HasForeignKey(ue => ue.ExpertiseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}