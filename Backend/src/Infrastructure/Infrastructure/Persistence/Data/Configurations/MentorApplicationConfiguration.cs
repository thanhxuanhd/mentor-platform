using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations
{
    public class MentorApplicationConfiguration : IEntityTypeConfiguration<MentorApplication>
    {
        public void Configure(EntityTypeBuilder<MentorApplication> builder)
        {
            builder.HasKey(ma => ma.Id);

            builder.Property(ma => ma.SubmittedAt)
                .IsRequired();

            builder.Property(ma => ma.ReviewedAt)
                .IsRequired(false);

            builder.Property(ma => ma.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(ApplicationStatus.Submitted);

            builder.Property(ma => ma.Statement)
                .HasMaxLength(300)
                .IsRequired(false);

            builder.Property(ma => ma.Note)
                .HasMaxLength(300)
                .IsRequired(false);

            builder.HasOne<User>(ma => ma.Mentor)
                .WithMany(u => u.MentorApplications)
                .HasForeignKey(ma => ma.MentorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ma => ma.ApplicationDocuments)
                .WithOne(ad => ad.MentorApplication)
                .HasForeignKey(ad => ad.MentorApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
