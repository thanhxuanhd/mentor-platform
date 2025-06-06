// filepath: Backend\src\Infrastructure\Infrastructure\Persistence\Data\Configurations\ScheduleConfiguration.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedules>
{
    public void Configure(EntityTypeBuilder<Schedules> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Mentor)
              .WithMany(u => u.Schedules)
              .HasForeignKey(s => s.MentorId)
              .OnDelete(DeleteBehavior.Cascade);

       builder.HasMany(s => s.AvailableTimeSlots)
              .WithOne(ts => ts.Schedules)
              .HasForeignKey(ts => ts.ScheduleId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.WeekStartDate) 
              .IsRequired();                 

        builder.Property(s => s.WeekEndDate)   
              .IsRequired();                 

        builder.Property(s => s.StartHour)
              .IsRequired();

        builder.Property(s => s.EndHour)
              .IsRequired();

        builder.Property(s => s.SessionDuration)
              .IsRequired();

        builder.Property(s => s.BufferTime)
              .IsRequired();
    }
}