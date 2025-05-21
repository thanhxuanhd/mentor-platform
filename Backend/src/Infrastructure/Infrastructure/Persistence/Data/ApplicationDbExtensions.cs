using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public static class ApplicationDbExtensions
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedAsync(dbContext, CancellationToken.None);
    }

    public static async Task SeedAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var categories = new List<Category>();
        var courses = new List<Course>();
        var courseTags = new List<CourseTag>();
        var tags = new List<Tag>();
        var courseStatus = new[] { CourseStatus.Published, CourseStatus.Draft, CourseStatus.Archived };
        var difficulties = new[]
            { CourseDifficulty.Beginner, CourseDifficulty.Intermediate, CourseDifficulty.Advanced };
        var random = new Random();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);
        
        if (!dbContext.Roles.Any())
        {
            dbContext.Roles.AddRange(new Role { Name = UserRole.Admin }, new Role { Name = UserRole.Mentor },
                new Role { Name = UserRole.Learner });

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var mentorRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Mentor);
        if (mentorRole is null) throw new Exception("User seeding: role name 'Mentor' does not exist in stores.");
        var learnerRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Learner);
        if (learnerRole is null) throw new Exception("User seeding: role name 'Learner' does not exist in stores.");

        if (!dbContext.Users.Any())
        {
            var users = new List<User>
            {
                new()
                {
                    FullName = "Dr. Sarah Chen", Email = "sarah.chen@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("Tech2025!"), RoleId = mentorRole.Id
                },
                new()
                {
                    FullName = "Prof. James Wilson", Email = "j.wilson@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("Edu2025#"), RoleId = mentorRole.Id
                },
                new()
                {
                    FullName = "Dr. Maria Garcia", Email = "m.garcia@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("Science2025@"), RoleId = mentorRole.Id
                },
                new()
                {
                    FullName = "Dr. David Kim", Email = "d.kim@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("Tech2025#"), RoleId = mentorRole.Id
                },
                new()
                {
                    FullName = "Prof. Emily Brown", Email = "e.brown@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("Edu2025$"), RoleId = mentorRole.Id
                },
                new()
                {
                    FullName = "Michael Chang", Email = "m.chang@learner.local",
                    PasswordHash = PasswordHelper.HashPassword("Learn2025$"), RoleId = learnerRole.Id
                },
                new()
                {
                    FullName = "Sofia Rodriguez", Email = "s.rodriguez@learner.local",
                    PasswordHash = PasswordHelper.HashPassword("Learn2025#"), RoleId = learnerRole.Id
                },
                new()
                {
                    FullName = "Alex Patel", Email = "a.patel@learner.local",
                    PasswordHash = PasswordHelper.HashPassword("Learn2025@"), RoleId = learnerRole.Id
                },
                new()
                {
                    FullName = "Emma Johnson", Email = "e.johnson@learner.local",
                    PasswordHash = PasswordHelper.HashPassword("Learn2025!"), RoleId = learnerRole.Id
                },
                new()
                {
                    FullName = "Lucas Zhang", Email = "l.zhang@learner.local",
                    PasswordHash = PasswordHelper.HashPassword("Learn2025^"), RoleId = learnerRole.Id
                },
                new()
                {
                    FullName = "Olivia Thompson", Email = "o.thompson@learner.local",
                    PasswordHash = PasswordHelper.HashPassword("Learn2025&"), RoleId = learnerRole.Id
                }
            };

            users.ForEach(u => u.Id = Guid.NewGuid());
            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.Categories.Any())
        {
            categories = new List<Category>
            {
                new()
                {
                    Name = "Full Stack Development",
                    Description = "Master both front-end and back-end development with modern frameworks and tools",
                    Status = true
                },
                new()
                {
                    Name = "Data Science & Analytics",
                    Description = "Learn data analysis, statistical modeling, and machine learning techniques",
                    Status = true
                },
                new()
                {
                    Name = "Cloud Architecture",
                    Description = "Design and implement scalable cloud solutions using AWS, Azure, and GCP",
                    Status = true
                },
                new()
                {
                    Name = "DevOps & Infrastructure",
                    Description = "Master CI/CD pipelines, containerization, and infrastructure automation",
                    Status = true
                },
                new()
                {
                    Name = "Cybersecurity",
                    Description = "Learn ethical hacking, security protocols, and threat mitigation strategies",
                    Status = true
                },
                new()
                {
                    Name = "Mobile Development",
                    Description = "Build native and cross-platform mobile applications for iOS and Android",
                    Status = true
                },
                new()
                {
                    Name = "AI & Machine Learning",
                    Description = "Develop intelligent systems using modern AI frameworks and deep learning",
                    Status = true
                },
                new()
                {
                    Name = "Blockchain Technology",
                    Description =
                        "Create decentralized applications and smart contracts on popular blockchain platforms",
                    Status = true
                },
                new()
                {
                    Name = "UI/UX Design",
                    Description = "Design intuitive user interfaces and optimize user experiences", Status = true
                },
                new()
                {
                    Name = "Quality Assurance",
                    Description = "Master software testing methodologies and automation frameworks", Status = true
                },
                new()
                {
                    Name = "System Architecture",
                    Description = "Design scalable, maintainable, and efficient software systems", Status = true
                }
            };

            categories.ForEach(c => c.Id = Guid.NewGuid());
            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.Courses.Any())
        {
            var mentorIds = await dbContext.Users.Where(u => u.RoleId == mentorRole.Id).Select(u => u.Id)
                .ToListAsync(cancellationToken);

            var courseTitles = new[]
            {
                "Modern Web Development with React and Node.js",
                "Machine Learning and Deep Neural Networks",
                "Cloud Architecture and Deployment Strategies",
                "DevOps Engineering and Automation",
                "Advanced Cybersecurity and Penetration Testing",
                "Cross-Platform Mobile App Development",
                "AI-Powered Systems and Applications",
                "Smart Contract Development with Solidity",
                "User Experience Design Principles",
                "Test Automation Best Practices",
                "Microservices Architecture Patterns"
            };

            var courseDescriptions = new[]
            {
                "Learn full-stack development using React and Node.js with real-world projects",
                "Master machine learning algorithms and deep learning frameworks",
                "Design and implement scalable cloud solutions across major platforms",
                "Implement CI/CD pipelines and automate infrastructure deployment",
                "Learn advanced security techniques and ethical hacking methodologies",
                "Build native and hybrid mobile apps using modern frameworks",
                "Create intelligent applications using cutting-edge AI technologies",
                "Develop secure and efficient smart contracts for blockchain platforms",
                "Master the art of creating intuitive and engaging user experiences",
                "Learn automated testing strategies and tools for quality assurance",
                "Design and implement scalable microservices architectures"
            };

            var staticCourseIds = new[]
            {
                Guid.Parse("2c330f36-9bf0-49dd-8ce9-c0c20cd0ddb6"),
                Guid.Parse("7a52d0f1-4c64-4d34-9076-36884ac4bc2b"),
                Guid.Parse("8e5b45c3-2f8d-4b5a-9a2e-77f2b3612f15"),
                Guid.Parse("9d4c8054-6a1e-4d6f-8e1d-89f1b3c6a2d3"),
                Guid.Parse("a3b7c9d8-e0f2-4a5b-9c6d-8f1e2d3b4a5c")
            };

            for (var i = 0; i < 11; i++)
            {
                var course = new Course
                {
                    Id = i < 5 ? staticCourseIds[i] : Guid.NewGuid(),
                    Title = courseTitles[i],
                    CategoryId = categories[i].Id,
                    MentorId = mentorIds[random.Next(mentorIds.Count)],
                    Status = courseStatus[random.Next(courseStatus.Length)],
                    DueDate = DateTime.UtcNow.AddMonths(random.Next(1, 7)),
                    Description = courseDescriptions[i],
                    Difficulty = difficulties[random.Next(difficulties.Length)]
                };
                courses.Add(course);
            }

            dbContext.Courses.AddRange(courses);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.Tags.Any())
        {
            tags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Web Development" },
                new() { Id = Guid.NewGuid(), Name = "Cloud Computing" },
                new() { Id = Guid.NewGuid(), Name = "Machine Learning" },
                new() { Id = Guid.NewGuid(), Name = "Cybersecurity" },
                new() { Id = Guid.NewGuid(), Name = "DevOps" },
                new() { Id = Guid.NewGuid(), Name = "Mobile Apps" },
                new() { Id = Guid.NewGuid(), Name = "Databases" },
                new() { Id = Guid.NewGuid(), Name = "Software Architecture" },
                new() { Id = Guid.NewGuid(), Name = "UI/UX" },
                new() { Id = Guid.NewGuid(), Name = "Testing" },
                new() { Id = Guid.NewGuid(), Name = "Blockchain" }
            };

            dbContext.Tags.AddRange(tags);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.CourseTags.Any())
        {
            foreach (var course in courses)
            {
                var tagCount = random.Next(2, 4);
                var courseTageSubset = tags.OrderBy(x => random.Next()).Take(tagCount);
                courseTags.AddRange(courseTageSubset.Select(tag => new CourseTag
                {
                    CourseId = course.Id,
                    TagId = tag.Id
                }));
            }

            dbContext.CourseTags.AddRange(courseTags);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.CourseItems.Any())
        {
            var mediaTypes = Enum.GetValues(typeof(CourseMediaType)).Cast<CourseMediaType>().ToList();
            var courseItems = new List<CourseItem>();

            var staticItemIds = new[]
            {
                new Guid("FFD36390-1CAA-4E4D-B507-7C21BD5903CD"),
                new Guid("241A2CB4-B87D-47F9-96DC-5A0F7362F60F"),
                new Guid("9D4DD0AD-8503-490D-9A9A-5916151863F4"),
                new Guid("DCF42DA3-19F8-40B5-9A70-A79D7A2B9B01"),
                new Guid("9B71977B-4050-4854-A269-474E4DB22C5A"),
            };
            var staticItemIndex = 0;

            foreach (var course in courses)
            {
                var moduleNames = new[]
                {
                    "Introduction and Setup",
                    "Core Concepts and Fundamentals",
                    "Advanced Techniques",
                    "Best Practices and Patterns",
                    "Real-world Application",
                    "Performance Optimization",
                    "Security Considerations",
                    "Testing and Quality Assurance",
                    "Deployment Strategies",
                    "Maintenance and Monitoring",
                    "Final Project and Review"
                };

                for (var i = 0; i < 11; i++)
                {
                    var mediaType = mediaTypes[random.Next(mediaTypes.Count)];
                    var item = new CourseItem
                    {
                        Id = staticItemIndex < staticItemIds.Length
                            ? staticItemIds[staticItemIndex++]
                            : Guid.NewGuid(),
                        Title = $"Module {i + 1}: {moduleNames[i]}",
                        Description =
                            $"Learn {moduleNames[i].ToLower()} in the context of {course.Title}",
                        MediaType = mediaType,
                        WebAddress = GetMediaUrl(mediaType, i + 1),
                        CourseId = course.Id
                    };
                    courseItems.Add(item);
                }
            }

            dbContext.CourseItems.AddRange(courseItems);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string GetMediaUrl(CourseMediaType mediaType, int moduleNumber)
    {
        return mediaType switch
        {
            CourseMediaType.Video => $"/content/videos/module{moduleNumber}.mp4",
            CourseMediaType.Pdf => $"/content/docs/module{moduleNumber}.pdf",
            CourseMediaType.ExternalWebAddress => $"https://learning.mentorplatform.local/module{moduleNumber}",
            _ => $"/content/other/module{moduleNumber}"
        };
    }
}