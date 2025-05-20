using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.FullName)
            .HasMaxLength(200);

        builder.Property(ud => ud.Bio)
            .HasMaxLength(500);

        builder.Property(ud => ud.ProfilePhotoUrl)
            .HasMaxLength(255);

        builder.Property(ud => ud.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ud => ud.Skills)
            .HasMaxLength(1000);

        builder.Property(ud => ud.Experiences)
            .HasMaxLength(500);

        builder.Property(ud => ud.Goal)
            .HasMaxLength(255);

        builder.Property(ud => ud.Availability)
            .HasConversion<string>();

        builder.Property(ud => ud.PreferredCommunicationMethod)
            .HasConversion<string>();

        builder.HasOne(user => user.Role)
            .WithMany(role => role.Users)
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}