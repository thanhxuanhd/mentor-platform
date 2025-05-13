using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public static class SeedData
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .HasData(new List<Role>
            {
                new() { Id = 1, Name = UserRole.Admin },
                new() { Id = 2, Name = UserRole.Mentor },
                new() { Id = 3, Name = UserRole.Learner }
            });
    }
}