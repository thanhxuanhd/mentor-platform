using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Data.Configurations;

public class MentorAvailableTimeSlotConfiguration : IEntityTypeConfiguration<MentorAvailableTimeSlot>
{
    public void Configure(EntityTypeBuilder<MentorAvailableTimeSlot> builder)
    {
        builder.HasKey(ts => ts.Id);
        builder.Property(ts => ts.Status)
            .HasConversion<string>();

        builder.Property(ts => ts.StartTime)
            .IsRequired();

        builder.Property(ts => ts.EndTime)
            .IsRequired();

        builder.HasOne(ts => ts.Mentor)
            .WithMany()
            .HasForeignKey(ts => ts.MentorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ts => ts.Schedule)
            .WithMany(s => s.TimeSlots)
            .HasForeignKey(ts => ts.ScheduleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}