using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.HasData(new List<Role>
        {
            new() { Id = 1, Name = UserRole.Admin },
            new() { Id = 2, Name = UserRole.Mentor },
            new() { Id = 3, Name = UserRole.Learner }
        });
    }
}