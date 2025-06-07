using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Status)
            .HasConversion<string>();

        builder.Property(c => c.Difficulty)
            .HasConversion<string>();

        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Mentor)
            .WithMany()
            .HasForeignKey(c => c.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Tags)
            .WithMany()
            .UsingEntity<CourseTag>();

        builder.HasMany(c => c.Resources)
            .WithOne(cr => cr.Course)
            .HasForeignKey(cr => cr.CourseId)
            .IsRequired();
    }
}