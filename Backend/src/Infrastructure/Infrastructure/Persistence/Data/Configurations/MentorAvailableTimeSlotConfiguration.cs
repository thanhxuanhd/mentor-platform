using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations
{
    public class MentorAvailableTimeSlotConfiguration : IEntityTypeConfiguration<MentorAvailableTimeSlot>
    {
        public void Configure(EntityTypeBuilder<MentorAvailableTimeSlot> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.StartTime)
                .IsRequired();

            builder.Property(t => t.EndTime)
                .IsRequired();

            builder.HasMany(t => t.Sessions)
                .WithOne(b => b.TimeSlot)
                .HasForeignKey(b => b.TimeSlotId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Schedules)
                .WithMany(s => s.AvailableTimeSlots)
                .HasForeignKey(t => t.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
