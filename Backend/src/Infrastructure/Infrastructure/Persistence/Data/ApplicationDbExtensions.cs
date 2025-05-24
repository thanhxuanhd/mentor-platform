using Application.Helpers;
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
            dbContext.Roles.AddRange(new Role { Name = UserRole.Admin }, new Role { Name = UserRole.Mentor },
                new Role { Name = UserRole.Learner });

            dbContext.SaveChanges();
        }

        if (!dbContext.Availabilities.Any())
        {
            dbContext.Availabilities.AddRange(
                new Availability { Name = "Weekdays" },
                new Availability { Name = "Weekends" },
                new Availability { Name = "Mornings" },
                new Availability { Name = "Afternoons" },
                new Availability { Name = "Evenings" }
            );

            dbContext.SaveChanges();
        }

        if (!dbContext.Expertises.Any())
        {
            dbContext.Expertises.AddRange(
                new Expertise { Name = "Leadership" },
                new Expertise { Name = "Programming" },
                new Expertise { Name = "Design" },
                new Expertise { Name = "Marketing" },
                new Expertise { Name = "Data Science" },
                new Expertise { Name = "Business" },
                new Expertise { Name = "Project Management" },
                new Expertise { Name = "Communication" }
            );

            dbContext.SaveChanges();
        }

        if (!dbContext.TeachingApproaches.Any())
        {
            dbContext.TeachingApproaches.AddRange(
                new TeachingApproach { Name = "Hands-on Practice" },
                new TeachingApproach { Name = "Discussion Based" },
                new TeachingApproach { Name = "Project Based" },
                new TeachingApproach { Name = "Lecture Style" }
            );

            dbContext.SaveChanges();
        }

        if (!dbContext.Users.Any())
        {
            var mentorRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Mentor);
            if (mentorRole is null) throw new Exception("User seeding: role name 'Mentor' does not exist in stores.");

            dbContext.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                    FullName = "MySuperKawawiiMentorXxX@at.local",
                    Email = "MySuperKawaiiMentorXxX@at.local",
                    PasswordHash = PasswordHelper.HashPassword("http://localhost:8080/register"),
                    RoleId = mentorRole.Id
                },
                new User
                {
                    Id = Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8"),
                    FullName = "DuongSenpai@at.local",
                    Email = "DuongSenpai@at.local",
                    PasswordHash = PasswordHelper.HashPassword("RollRoyce420$$$"),
                    RoleId = mentorRole.Id
                },
                new User
                {
                    Id = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"),
                    FullName = "AnhDoSkibidi@at.local",
                    Email = "AnhDoSkibidi@at.local",
                    PasswordHash = PasswordHelper.HashPassword("!@#$%^&*{%item_04%}"),
                    RoleId = mentorRole.Id
                }
            );

            dbContext.SaveChanges();
        }

        if (!dbContext.Categories.Any())
        {
            dbContext.Categories.AddRange(new Category
            {
                Id = Guid.Parse("3144da58-deaa-4bf7-a777-cd96e7f1e3b1"), Name = "Leadership Coaching",
                Description = "Courses related to developing leadership skills and strategies", Status = true
            }, new Category
            {
                Id = Guid.Parse("07e80bb4-5fbb-4016-979d-847878ab81d5"), Name = "Communication Skills",
                Description = "Effective communication in professional settings", Status = true
            }, new Category
            {
                Id = Guid.Parse("4aa8eb25-7bb0-4bdc-b391-9924bc218eb2"), Name = "Public Speaking",
                Description = "Techniques to improve public speaking and presentation skills", Status = true
            }, new Category
            {
                Id = Guid.Parse("4b896130-3727-46c7-98d1-214107bd4709"), Name = "Time Management",
                Description = "Strategies for better time management and productivity", Status = false
            }, new Category
            {
                Id = Guid.Parse("ead230f7-76ff-4c10-b025-d1f80fcdd277"), Name = "Career Development",
                Description = "Resources for career advancement and job hunting", Status = false
            });

            dbContext.SaveChanges();
        }

        if (!dbContext.Courses.Any())
        {
            dbContext.Courses.AddRange(new Course
            {
                Id = Guid.Parse("b5ffe7dc-ead8-4072-84fc-2aa39908fffe"),
                Title = "Introduction to Leadership",
                CategoryId = Guid.Parse("3144da58-deaa-4bf7-a777-cd96e7f1e3b1"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = CourseStatus.Published,
                DueDate = DateTime.UtcNow.AddMonths(3),
                Description = "Learn the principles of effective leadership.",
                Difficulty = CourseDifficulty.Beginner
            }, new Course
            {
                Id = Guid.Parse("e262d134-e6f3-48d3-83b0-4bedf783aa8f"),
                Title = "Advanced Communication Techniques",
                CategoryId = Guid.Parse("07e80bb4-5fbb-4016-979d-847878ab81d5"),
                MentorId = Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8"),
                Status = CourseStatus.Draft,
                DueDate = DateTime.UtcNow.AddMonths(2),
                Description = "Master advanced communication skills for the workplace.",
                Difficulty = CourseDifficulty.Intermediate
            }, new Course
            {
                Id = Guid.Parse("08ab0125-927c-43b5-8263-7ebaab51c18a"),
                Title = "Public Speaking Mastery",
                CategoryId = Guid.Parse("4aa8eb25-7bb0-4bdc-b391-9924bc218eb2"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = CourseStatus.Published,
                DueDate = DateTime.UtcNow.AddMonths(1),
                Description = "Become a confident public speaker.",
                Difficulty = CourseDifficulty.Advanced
            }, new Course
            {
                Id = Guid.Parse("2c330f36-9bf0-49dd-8ce9-c0c20cd0ddb6"),
                Title = "Time Management for Professionals",
                CategoryId = Guid.Parse("4b896130-3727-46c7-98d1-214107bd4709"),
                MentorId = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"),
                Status = CourseStatus.Draft,
                DueDate = DateTime.UtcNow.AddMonths(4),
                Description = "Learn effective time management strategies.",
                Difficulty = CourseDifficulty.Beginner
            }, new Course
            {
                Id = Guid.Parse("621c9cf6-aa10-40c8-aace-2d649a261a4a"),
                Title = "Effective Team Leadership",
                CategoryId = Guid.Parse("3144da58-deaa-4bf7-a777-cd96e7f1e3b1"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = CourseStatus.Archived,
                DueDate = DateTime.UtcNow.AddMonths(5),
                Description = "Learn how to lead and manage teams effectively.",
                Difficulty = CourseDifficulty.Advanced
            });

            dbContext.SaveChanges();
        }

        if (!dbContext.Tags.Any())
        {
            dbContext.Tags.AddRange(
                new Tag { Id = Guid.Parse("1f5c7b87-a572-46b7-9ed2-7be81520fff2"), Name = "Leadership" },
                new Tag { Id = Guid.Parse("c13eafac-c4d5-445c-87f1-393c40f90d08"), Name = "Communication" },
                new Tag { Id = Guid.Parse("4e21ccd5-5b36-4f2b-9472-d1ab4cf95ab6"), Name = "Public Speaking" },
                new Tag { Id = Guid.Parse("66382d29-a177-4d1b-b6cf-747ccea33bce"), Name = "Time Management" },
                new Tag { Id = Guid.Parse("3a6c27f3-1518-4575-8790-54764c2851a7"), Name = "Career Development" });

            dbContext.SaveChanges();
        }

        if (!dbContext.CourseTags.Any())
        {
            dbContext.CourseTags.AddRange(new CourseTag
            {
                CourseId = Guid.Parse("b5ffe7dc-ead8-4072-84fc-2aa39908fffe"),
                TagId = Guid.Parse("1f5c7b87-a572-46b7-9ed2-7be81520fff2")
            }, new CourseTag
            {
                CourseId = Guid.Parse("b5ffe7dc-ead8-4072-84fc-2aa39908fffe"),
                TagId = Guid.Parse("c13eafac-c4d5-445c-87f1-393c40f90d08")
            }, new CourseTag
            {
                CourseId = Guid.Parse("e262d134-e6f3-48d3-83b0-4bedf783aa8f"),
                TagId = Guid.Parse("4e21ccd5-5b36-4f2b-9472-d1ab4cf95ab6")
            }, new CourseTag
            {
                CourseId = Guid.Parse("08ab0125-927c-43b5-8263-7ebaab51c18a"),
                TagId = Guid.Parse("66382d29-a177-4d1b-b6cf-747ccea33bce")
            }, new CourseTag
            {
                CourseId = Guid.Parse("2c330f36-9bf0-49dd-8ce9-c0c20cd0ddb6"),
                TagId = Guid.Parse("1f5c7b87-a572-46b7-9ed2-7be81520fff2")
            });

            dbContext.SaveChanges();
        }

        return app;
    }
}