using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations
{
    internal class TimeSlotConfiguration : IEntityTypeConfiguration<MentorAvailableTimeSlot>
    {
        public void Configure(EntityTypeBuilder<MentorAvailableTimeSlot> builder)
        {
            builder.HasKey(t => t.Id);

            builder
                .HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(t => t.Schedule)
                .WithMany()
                .HasForeignKey(t => t.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.StartTime)
                .IsRequired();

            builder.Property(t => t.EndTime)
                .IsRequired();
        }
    }
}
