using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(b => b.TimeSlot)
            .WithMany(ts => ts.Bookings)
            .HasForeignKey(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Learner)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.LearnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
