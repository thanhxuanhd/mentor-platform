using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class CourseResourceConfiguration : IEntityTypeConfiguration<CourseResource>
{
    public void Configure(EntityTypeBuilder<CourseResource> builder)
    {
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cr => cr.Description)
            .HasMaxLength(1024);

        builder.Property(cr => cr.ResourceType)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(FileType.Pdf);

        builder.Property(cr => cr.ResourceUrl)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(cr => cr.ResourceUrl)
            .IsUnique();

        builder.HasOne(cr => cr.Course)
            .WithMany(c => c.Resources)
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}