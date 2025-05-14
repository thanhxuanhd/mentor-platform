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

        if (!dbContext.Categories.Any())
        {
            dbContext.Categories.AddRange([
                new Category { Name = "Leadership Coaching", Description = "Courses related to developing leadership skills and strategies", Status = true },
                new Category { Name = "Communication Skills", Description = "Effective communication in professional settings", Status = true },
                new Category { Name = "Public Speaking", Description = "Techniques to improve public speaking and presentation skills", Status = true },
                new Category { Name = "Time Management", Description = "Strategies for better time management and productivity", Status = false },
                new Category { Name = "Career Development", Description = "Resources for career advancement and job hunting", Status = false }
            ]);

            dbContext.SaveChanges();
        }

        if (!dbContext.Courses.Any())
        {
            dbContext.Courses.AddRange([
                new Course
                {
                    Title = "Introduction to Leadership",
                    CategoryId = 1,
                    Status = CourseStatus.Published,
                    DueDate = DateTime.UtcNow.AddMonths(3),
                    Description = "Learn the principles of effective leadership.",
                    Difficulty = CourseDifficulty.Beginner
                },
                new Course
                {
                    Title = "Advanced Communication Techniques",
                    CategoryId = 2,
                    Status = CourseStatus.Draft,
                    DueDate = DateTime.UtcNow.AddMonths(2),
                    Description = "Master advanced communication skills for the workplace.",
                    Difficulty = CourseDifficulty.Intermediate
                },
                new Course
                {
                    Title = "Public Speaking Mastery",
                    CategoryId = 3,
                    Status = CourseStatus.Published,
                    DueDate = DateTime.UtcNow.AddMonths(1),
                    Description = "Become a confident public speaker.",
                    Difficulty = CourseDifficulty.Advanced
                },
                new Course
                {
                    Title = "Time Management for Professionals",
                    CategoryId = 4,
                    Status = CourseStatus.Draft,
                    DueDate = DateTime.UtcNow.AddMonths(4),
                    Description = "Learn effective time management strategies.",
                    Difficulty = CourseDifficulty.Beginner
                },
                new Course
                {
                    Title = "Effective Team Leadership",
                    CategoryId = 1,
                    Status = CourseStatus.Archived,
                    DueDate = DateTime.UtcNow.AddMonths(5),
                    Description = "Learn how to lead and manage teams effectively.",
                    Difficulty = CourseDifficulty.Advanced
                }
            ]);

            dbContext.SaveChanges();
        }

        return app;
    }
}