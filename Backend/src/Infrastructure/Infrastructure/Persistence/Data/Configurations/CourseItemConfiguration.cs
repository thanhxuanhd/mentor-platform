using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class CourseItemConfiguration : IEntityTypeConfiguration<CourseItem>
{
    public void Configure(EntityTypeBuilder<CourseItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(ci => ci.Description)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(ci => ci.MediaType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ci => ci.WebAddress)
            .IsRequired()
            .HasMaxLength(2048);

        builder.HasOne(ci => ci.Course)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}