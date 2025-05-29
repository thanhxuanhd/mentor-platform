using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.SessionType)
            .HasConversion<string>();

        builder.Property(b => b.Status)
            .HasConversion<string>();

        builder.Property(b => b.BookedDateTime)
            .IsRequired();

        builder.HasOne(b => b.TimeSlot)
            .WithMany(ts => ts.Bookings)
            .HasForeignKey(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}