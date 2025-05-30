using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations
{
    public class TimeSlotConfiguration : IEntityTypeConfiguration<MentorAvailableTimeSlot>
    {
        public void Configure(EntityTypeBuilder<MentorAvailableTimeSlot> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.StartTime)
                .IsRequired();

            builder.Property(t => t.EndTime)
                .IsRequired();

            builder.HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Bookings)
                .WithOne(b => b.TimeSlot)
                .HasForeignKey(b => b.TimeSlotId)
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
