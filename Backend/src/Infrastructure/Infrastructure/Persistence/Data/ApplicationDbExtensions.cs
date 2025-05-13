using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Data;

public static class ApplicationDbExtensions
{
    public static IApplicationBuilder SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        dbContext!.Database.EnsureCreated();
        if (!dbContext.Roles.Any())
        {
            dbContext.Roles.AddRange([
                new Role { Name = UserRole.Admin },
                new Role { Name = UserRole.Mentor },
                new Role { Name = UserRole.Learner }
            ]);

            dbContext.SaveChanges();
        }

        return app;
    }
}