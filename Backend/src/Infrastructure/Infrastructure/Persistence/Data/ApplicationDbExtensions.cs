using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;

namespace Infrastructure.Persistence.Data;

public static class ApplicationDbExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        dbContext!.Database.EnsureCreated();

        // Seeding logic moved directly here
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
            var learnerRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Learner);
            var adminRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Admin);
            if (mentorRole is null)
            {
                throw new Exception("User seeding: role name 'Mentor' does not exist in stores.");
            }

            if (learnerRole is null)
            {
                throw new Exception("User seeding: role name 'Learner' does not exist in stores.");
            }

            if (adminRole is null)
            {
                throw new Exception("User seeding: role name 'Admin' does not exist in stores.");
            }

            dbContext.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                    FullName = "Alice Wonderland",
                    Email = "alice.wonderland@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("password@123"),
                    RoleId = mentorRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8"),
                    FullName = "Bob Builder",
                    Email = "bob.builder@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("securepassword@345"),
                    RoleId = mentorRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"),
                    FullName = "Charlie Chaplin",
                    Email = "charlie.chaplin@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("anotherpassword@345"),
                    RoleId = mentorRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("547a020b-86e9-4713-a17d-ded22a84bda1"),
                    FullName = "John Doe",
                    Email = "johndoe@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("anotherpassword@345"),
                    RoleId = mentorRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("F09BDC14-081D-4C73-90A7-4CDB38BF176C"),
                    FullName = "David Copperfield",
                    Email = "david.copperfield@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("mypassword@123"),
                    RoleId = learnerRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("831A3848-7D77-4BE0-958B-4EFE064752F1"),
                    FullName = "The Administrator",
                    Email = "mini@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("mypassword88$"),
                    RoleId = adminRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("C10C1A72-6B7D-4F60-84E9-3F63353A81A1"),
                    FullName = "Lerbon James",
                    Email = "nguyenvana@mentorplatform.local",
                    PasswordHash = PasswordHelper.HashPassword("passA123"),
                    RoleId = learnerRole.Id,
                    Status = UserStatus.Active
                },
                new User
                {
                    Id = Guid.Parse("D21D2B83-7C8E-4071-95F0-4C74464B92B2"),
                    FullName = "Stephen Curry",
                    Email = "ngominh24122001@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("passB123"),
                    RoleId = learnerRole.Id,
                    Status = UserStatus.Active
                }
            );
            dbContext.SaveChanges();
        }

        if (!dbContext.Categories.Any())
        {
            dbContext.Categories.AddRange(new Category
            {
                Id = Guid.Parse("3144da58-deaa-4bf7-a777-cd96e7f1e3b1"),
                Name = "Leadership Coaching",
                Description = "Courses related to developing leadership skills and strategies",
                Status = true
            }, new Category
            {
                Id = Guid.Parse("07e80bb4-5fbb-4016-979d-847878ab81d5"),
                Name = "Communication Skills",
                Description = "Effective communication in professional settings",
                Status = true
            }, new Category
            {
                Id = Guid.Parse("4aa8eb25-7bb0-4bdc-b391-9924bc218eb2"),
                Name = "Public Speaking",
                Description = "Techniques to improve public speaking and presentation skills",
                Status = true
            }, new Category
            {
                Id = Guid.Parse("4b896130-3727-46c7-98d1-214107bd4709"),
                Name = "Time Management",
                Description = "Strategies for better time management and productivity",
                Status = false
            }, new Category
            {
                Id = Guid.Parse("ead230f7-76ff-4c10-b025-d1f80fcdd277"),
                Name = "Career Development",
                Description = "Resources for career advancement and job hunting",
                Status = false
            });
            dbContext.SaveChanges();
        }

        if (!dbContext.Courses.Any())
        {
            dbContext.Courses.AddRange(new Course
            {
                Id = Guid.Parse("b5ffe7dc-ead8-4072-84fc-2aa39908fffe"),
                Title = "Foundations of Effective Leadership",
                CategoryId = Guid.Parse("3144da58-deaa-4bf7-a777-cd96e7f1e3b1"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = CourseStatus.Published,
                DueDate = DateTime.UtcNow.AddMonths(3),
                Description = "Explore the fundamental principles and practices of effective leadership.",
                Difficulty = CourseDifficulty.Beginner
            }, new Course
            {
                Id = Guid.Parse("e262d134-e6f3-48d3-83b0-4bedf783aa8f"),
                Title = "Mastering Business Communication",
                CategoryId = Guid.Parse("07e80bb4-5fbb-4016-979d-847878ab81d5"),
                MentorId = Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8"),
                Status = CourseStatus.Draft,
                DueDate = DateTime.UtcNow.AddMonths(2),
                Description = "Develop advanced skills in professional communication for various business contexts.",
                Difficulty = CourseDifficulty.Intermediate
            }, new Course
            {
                Id = Guid.Parse("08ab0125-927c-43b5-8263-7ebaab51c18a"),
                Title = "Confident Public Speaking",
                CategoryId = Guid.Parse("4aa8eb25-7bb0-4bdc-b391-9924bc218eb2"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = CourseStatus.Published,
                DueDate = DateTime.UtcNow.AddMonths(1),
                Description = "Build confidence and refine your techniques for impactful public speaking.",
                Difficulty = CourseDifficulty.Advanced
            }, new Course
            {
                Id = Guid.Parse("2c330f36-9bf0-49dd-8ce9-c0c20cd0ddb6"),
                Title = "Productivity with Time Management",
                CategoryId = Guid.Parse("4b896130-3727-46c7-98d1-214107bd4709"),
                MentorId = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"),
                Status = CourseStatus.Draft,
                DueDate = DateTime.UtcNow.AddMonths(4),
                Description =
                    "Learn and apply proven strategies to manage your time effectively and increase productivity.",
                Difficulty = CourseDifficulty.Beginner
            }, new Course
            {
                Id = Guid.Parse("621c9cf6-aa10-40c8-aace-2d649a261a4a"),
                Title = "Leading High-Performing Teams",
                CategoryId = Guid.Parse("3144da58-deaa-4bf7-a777-cd96e7f1e3b1"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = CourseStatus.Archived,
                DueDate = DateTime.UtcNow.AddMonths(5),
                Description = "Discover how to build, motivate, and lead high-performing teams.",
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

        if (!dbContext.MentorApplications.Any())
        {
            dbContext.MentorApplications.AddRange(new MentorApplication
            {
                Id = Guid.Parse("003919e7-16ca-471b-af9c-02ca5f31dbb6"),
                MentorId = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"),
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow,
                ReviewedAt = null,
                Education = "Bachelor's in Computer Science",
                Certifications = "Certified Scrum Master, AWS Certified Solutions Architect",
                Statement = null,
                Note = null
            },
            new MentorApplication
            {
                Id = Guid.Parse("7ad56b17-cb7d-4ca8-9058-8a05def6229f"),
                MentorId = Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8"),
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow,
                ReviewedAt = null,
                Education = "Master's in Business Administration",
                Certifications = "Project Management Professional (PMP), Six Sigma Green Belt",
                Statement = null,
                Note = null
            },
            new MentorApplication
            {
                Id = Guid.Parse("078c407e-31d6-4bf6-93e5-b794123e1177"),
                MentorId = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"),
                Status = ApplicationStatus.Rejected,
                SubmittedAt = new DateTime(2000, 1, 1),
                ReviewedAt = null,
                Education = "Bachelor's in Arts",
                Certifications = "Certified Art Therapist",
                Statement = null,
                Note = "Invalid"
            },
            new MentorApplication
            {
                Id = Guid.Parse("c9ad54d7-f90e-4ecf-b8ec-8b5badedf171"),
                MentorId = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"),
                Status = ApplicationStatus.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-10),
                ReviewedAt = DateTime.UtcNow,
                AdminId = Guid.Parse("831A3848-7D77-4BE0-958B-4EFE064752F1"),
                Education = "Bachelor's in Graphic Design",
                Certifications = null,
                Statement = null,
                Note = null
            });
            dbContext.SaveChanges();
        }

        var scheduleId = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
        if (!dbContext.Schedules.Any())
        {
            var mentor1Id = Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE");
            var mentor2Id = Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            dbContext.Schedules.AddRange(
                new Schedules
                {
                    Id = scheduleId,
                    MentorId = mentor1Id,
                    WeekStartDate = startOfWeek,
                    WeekEndDate = endOfWeek,
                    StartHour = new TimeOnly(09, 00),
                    EndHour = new TimeOnly(17, 00),
                    SessionDuration = 60,
                    BufferTime = 15,
                },
                new Schedules
                {
                    MentorId = mentor1Id,
                    WeekStartDate = DateOnly.FromDateTime(new DateTime(2025, 5, 25)),
                    WeekEndDate = DateOnly.FromDateTime(new DateTime(2025, 5, 31)),
                    StartHour = new TimeOnly(09, 00),
                    EndHour = new TimeOnly(17, 00),
                    SessionDuration = 60,
                    BufferTime = 15,
                },
                new Schedules
                {
                    MentorId = mentor2Id,
                    WeekStartDate = DateOnly.FromDateTime(new DateTime(2025, 5, 25)),
                    WeekEndDate = DateOnly.FromDateTime(new DateTime(2025, 5, 31)),
                    StartHour = new TimeOnly(13, 00),
                    EndHour = new TimeOnly(21, 00),
                    SessionDuration = 30,
                    BufferTime = 5,
                },
                new Schedules
                {
                    MentorId = mentor2Id,
                    WeekStartDate = DateOnly.FromDateTime(new DateTime(2025, 6, 1)),
                    WeekEndDate = DateOnly.FromDateTime(new DateTime(2025, 6, 7)),
                    StartHour = new TimeOnly(09, 00),
                    EndHour = new TimeOnly(12, 00),
                    SessionDuration = 60,
                    BufferTime = 0,
                }
            );
            dbContext.SaveChanges();
        }

        if (!dbContext.Schedules.Any(s => s.MentorId == Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C")))
        {
            var charlieScheduleId = Guid.NewGuid();
            var charlieMentorId = Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            dbContext.Schedules.Add(new Schedules
            {
                Id = charlieScheduleId,
                MentorId = charlieMentorId,
                WeekStartDate = startOfWeek,
                WeekEndDate = endOfWeek,
                StartHour = new TimeOnly(08, 00), 
                EndHour = new TimeOnly(18, 00),  
                SessionDuration = 45,             
                BufferTime = 10,                  
            });
            dbContext.SaveChanges();

            var charlieTimeSlots = new List<MentorAvailableTimeSlot>();
            var random = new Random();

            for (int week = 0; week < 2; week++)
            {
                for (int day = 1; day <= 5; day++) 
                {
                    var slotDate = today.AddDays((week * 7) + day);

                    var slotsPerDay = random.Next(3, 5);
                    for (int slot = 0; slot < slotsPerDay; slot++)
                    {
                        var startHour = 8 + (slot * 2);
                        if (startHour >= 18) break; 

                        charlieTimeSlots.Add(new MentorAvailableTimeSlot
                        {
                            Id = Guid.NewGuid(),
                            ScheduleId = charlieScheduleId,
                            Date = slotDate,
                            StartTime = new TimeOnly(startHour, 0),
                            EndTime = new TimeOnly(startHour + 1, 0), 
                        });
                    }
                }
            }

            for (int week = 0; week < 2; week++)
            {
                var saturdayDate = today.AddDays((week * 7) + 6);
                charlieTimeSlots.Add(new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = charlieScheduleId,
                    Date = saturdayDate,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(10, 0),
                });

                charlieTimeSlots.Add(new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = charlieScheduleId,
                    Date = saturdayDate,
                    StartTime = new TimeOnly(14, 0),
                    EndTime = new TimeOnly(15, 0),
                });

                var sundayDate = today.AddDays((week * 7) + 7);
                charlieTimeSlots.Add(new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = charlieScheduleId,
                    Date = sundayDate,
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(11, 0),
                });
            }

            dbContext.MentorAvailableTimeSlots.AddRange(charlieTimeSlots);
            dbContext.SaveChanges();
        }

        if (!dbContext.MentorAvailableTimeSlots.Any())
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var existingSchedule = dbContext.Schedules.FirstOrDefault(s => s.Id == scheduleId);
            if (existingSchedule == null)
            {
                throw new Exception("Schedule not found for time slot seeding");
            }

            var timeSlots = new List<MentorAvailableTimeSlot>();

            for (int i = 0; i < 7; i++)
            {
                timeSlots.Add(new MentorAvailableTimeSlot
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    Date = today.AddDays(i + 1),
                    StartTime = new TimeOnly(9 + i, 0),
                    EndTime = new TimeOnly(10 + i, 0),
                });
            }

            dbContext.MentorAvailableTimeSlots.AddRange(timeSlots);
            dbContext.SaveChanges();

            var createdSlots = dbContext.MentorAvailableTimeSlots.Count();
        }
        if (!dbContext.Sessions.Any())
        {
            dbContext.Sessions.RemoveRange(dbContext.Sessions);
            dbContext.SaveChanges();

            var mentorIds = new List<Guid>
            {
                Guid.Parse("BC7CB279-B292-4CA3-A994-9EE579770DBE"), 
                Guid.Parse("B5095B17-D0FE-47CC-95B8-FD7E560926F8"), 
                Guid.Parse("01047F62-6E87-442B-B1E8-2A54C9E17D7C"), 
                Guid.Parse("547a020b-86e9-4713-a17d-ded22a84bda1"),
            };

                    var learnerIds = new List<Guid>
            {
                Guid.Parse("F09BDC14-081D-4C73-90A7-4CDB38BF176C"),
                Guid.Parse("C10C1A72-6B7D-4F60-84E9-3F63353A81A1"),
                Guid.Parse("D21D2B83-7C8E-4071-95F0-4C74464B92B2")
            };

            var sessionsToAdd = new List<Sessions>();
            var random = new Random();

            foreach (var mentorId in mentorIds)
            {
                var schedules = dbContext.Schedules
                    .Where(s => s.MentorId == mentorId)
                    .Select(s => s.Id)
                    .ToList();

                if (!schedules.Any()) continue;

                var availableTimeSlots = dbContext.MentorAvailableTimeSlots
                    .Where(ts => schedules.Contains(ts.ScheduleId))
                    .OrderBy(ts => ts.Date)
                    .ThenBy(ts => ts.StartTime)
                    .Take(10)
                    .ToList();

                foreach (var timeSlot in availableTimeSlots)
                {
                    var learnerId = learnerIds[random.Next(learnerIds.Count)];
                    sessionsToAdd.Add(new Sessions
                    {
                        Id = Guid.NewGuid(),
                        LearnerId = learnerId,
                        TimeSlotId = timeSlot.Id,
                        Status = SessionStatus.Pending,
                        Type = SessionType.Onsite,
                        BookedOn = DateTime.UtcNow.AddMinutes(-random.Next(60, 720))
                    });
                }
            }

            dbContext.Sessions.AddRange(sessionsToAdd);
            dbContext.SaveChanges();

        }

    }
}