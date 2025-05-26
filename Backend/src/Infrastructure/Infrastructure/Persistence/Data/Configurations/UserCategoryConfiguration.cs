using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserCategoryConfiguration : IEntityTypeConfiguration<UserCategory>
{
    public void Configure(EntityTypeBuilder<UserCategory> builder)
    {
        builder.HasKey(ue => ue.Id);

        builder.HasIndex(ue => new { ue.UserId, ue.CategoryId })
            .IsUnique();

        builder.HasOne(ue => ue.User)
            .WithMany(u => u.UserCategories)
            .HasForeignKey(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Category)
            .WithMany(u => u.UserCategories)
            .HasForeignKey(ue => ue.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}