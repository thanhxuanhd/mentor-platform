using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    // TODO: explicitly setting all entity as optional, await team member to complete their parts
    // Set all property as required once blocking items got resolved
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
        
        // TODO: https://github.com/dotnet/efcore/issues/13146, https://github.com/dotnet/efcore/issues/15854
        // Solution: raise issue and wait for user seeding 
        // builder.HasOne(c => c.Mentor)
        //     .WithMany()
        //     .HasForeignKey(c => c.MentorId);
        
        builder.HasMany(c => c.Tags)
            .WithMany()
            .UsingEntity<CourseTag>();
    }
}