using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint("CK_PreferredSessionDuration", "\"PreferredSessionDuration\" IN (30, 45, 60, 90, 120)"));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(u => u.Status)
            .HasDefaultValue(UserStatus.Pending);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasDefaultValue("")
            .HasMaxLength(100);

        builder.Property(u => u.Timezone)
            .IsRequired()
            .HasDefaultValue("Asia/Bangkok")
            .HasMaxLength(100);

        builder.Property(u => u.Bio)
            .HasMaxLength(300);

        builder.Property(u => u.ProfilePhotoUrl)
            .HasMaxLength(200);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasDefaultValue("")
            .HasMaxLength(10);

        builder.Property(u => u.Skills)
            .HasMaxLength(200);

        builder.Property(u => u.Experiences)
            .HasMaxLength(200);

        builder.Property(u => u.PreferredCommunicationMethod)
            .HasDefaultValue(CommunicationMethod.VideoCall)
            .HasConversion<string>()
            .IsRequired(true);

        builder.Property(u => u.Goal)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(u => u.PreferredSessionFrequency)
            .HasConversion<string>()
            .HasDefaultValue(SessionFrequency.AsNeeded)
            .IsRequired();

        builder.Property(u => u.PreferredSessionDuration)
            .HasDefaultValue(30)
            .IsRequired();

        builder.Property(u => u.PreferredLearningStyle)
            .HasConversion<string>()
            .HasDefaultValue(LearningStyle.Visual)
            .IsRequired();

        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.MentorApplications)
            .WithOne(ma => ma.Mentor)
            .HasForeignKey(ma => ma.MentorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.ReviewedMentorApplications)
            .WithOne(ma => ma.Admin)
            .HasForeignKey(ma => ma.AdminId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.Schedules)
            .WithOne(s => s.Mentor) 
            .HasForeignKey(s => s.MentorId) 
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Sessions)
            .WithOne(b => b.Learner) 
            .HasForeignKey(b => b.LearnerId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}