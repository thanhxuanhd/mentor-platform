using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
{
    public void Configure(EntityTypeBuilder<UserDetail> builder)
    {
        builder.HasKey(ud => ud.Id);

        builder.Property(ud => ud.UserId)
            .IsRequired();

        builder.Property(ud => ud.FullName)
            .IsRequired()
            .HasMaxLength(100);

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

        builder.Property(ud => ud.ProfileCompleteStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ud => ud.Availability)
            .HasConversion<string>();

        builder.Property(ud => ud.PreferredCommunicationMethod)
            .HasConversion<string>();

        builder.HasOne(ud => ud.User)
            .WithOne(u => u.UserDetail)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}