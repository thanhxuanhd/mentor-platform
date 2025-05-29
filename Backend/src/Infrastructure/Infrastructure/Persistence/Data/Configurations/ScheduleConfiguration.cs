using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.MentorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.SessionDuration)
            .IsRequired();

        builder.Property(s => s.BufferTime)
            .IsRequired();

        builder.Property(s => s.IsLocked)
            .HasDefaultValue(false);

        builder.HasIndex(s => new { s.MentorId, s.Day });

        builder.HasQueryFilter(s => !s.IsLocked);
    }
}