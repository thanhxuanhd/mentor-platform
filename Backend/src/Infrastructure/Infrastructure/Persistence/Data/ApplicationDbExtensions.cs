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

        if (!dbContext.Users.Any())
        {
            var mentorRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Mentor);
            if (mentorRole is null) throw new Exception("User seeding: role name 'Mentor' does not exist in stores.");

            var adminRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Admin);
            if (adminRole is null) throw new Exception("User seeding: role name 'Mentor' does not exist in stores.");

            var learnerRole = dbContext.Roles.FirstOrDefault(r => r.Name == UserRole.Learner);
            if (learnerRole is null) throw new Exception("User seeding: role name 'Mentor' does not exist in stores.");

            dbContext.Users.AddRange(

    new User { Id = Guid.Parse("2f1f83f1-7bfa-4f63-b89d-f79fd71a9e01"), FullName = "Tran Bach", Email = "bachtran@gmail.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 1, 1), LastActive = new DateOnly(2025, 1, 10), Status = UserStatus.Active },
    new User { Id = Guid.Parse("d4e145e9-9d64-403d-90fd-34f0b7bc7ac2"), FullName = "Nguyen Lan", Email = "lan.nguyen@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 2, 1), LastActive = new DateOnly(2025, 2, 1), Status = UserStatus.Active },
    new User { Id = Guid.Parse("c47ae4b5-1e30-4cc9-9eb4-987e4d5f0c9b"), FullName = "Pham Hoang", Email = "hoang.pham@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 2, 15), LastActive = new DateOnly(2025, 2, 20), Status = UserStatus.Active },
    new User { Id = Guid.Parse("e0b177d3-33d9-4b6a-91d6-ea927f84e8b1"), FullName = "Le Hien", Email = "hien.le@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 3, 1), LastActive = new DateOnly(2025, 3, 1), Status = UserStatus.Active },
    new User { Id = Guid.Parse("7c6b3880-b879-4b84-8bfb-958f53f8a0ec"), FullName = "Dang Minh", Email = "minh.dang@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 3, 15), LastActive = new DateOnly(2025, 3, 20), Status = UserStatus.Active },
    new User { Id = Guid.Parse("9f41c5c6-7326-46cb-8506-6b2c3d3b138e"), FullName = "Vo Quynh", Email = "quynh.vo@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 4, 1), LastActive = new DateOnly(2025, 4, 1), Status = UserStatus.Active },
    new User { Id = Guid.Parse("15e01211-0f1f-435f-8a3a-74c73b1cbb13"), FullName = "Nguyen Tuan", Email = "tuan.nguyen@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 4, 15), LastActive = new DateOnly(2025, 4, 20), Status = UserStatus.Active },
    new User { Id = Guid.Parse("fc1d3764-4b7c-4b5b-8a50-e50653f62770"), FullName = "Tran Ha", Email = "ha.tran@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 5, 1), LastActive = new DateOnly(2025, 5, 1), Status = UserStatus.Active },
    new User { Id = Guid.Parse("55c149e0-3098-4370-bff5-e474e5be3c74"), FullName = "Hoang Chau", Email = "chau.hoang@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 5, 15), LastActive = new DateOnly(2025, 5, 10), Status = UserStatus.Active },
    new User { Id = Guid.Parse("1ad76a2e-d7ae-4d2c-98b7-08f36e3a20ee"), FullName = "Le Bao", Email = "bao.le@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 6, 1), LastActive = new DateOnly(2025, 5, 15), Status = UserStatus.Active },

    // PENDING USERS
    new User { Id = Guid.Parse("b1b5df53-58fd-4201-a799-720af9f20629"), FullName = "Ngo Hieu", Email = "hieu.ngo@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 6, 15), LastActive = new DateOnly(2024, 6, 1), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("0bc1fc36-4c74-4234-b273-3dd0ed358482"), FullName = "Phan Linh", Email = "linh.phan@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 7, 1), LastActive = new DateOnly(2024, 7, 1), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("fb84c5de-0b99-4cf6-b4f3-98782e2b0f83"), FullName = "Mai An", Email = "an.mai@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 7, 15), LastActive = new DateOnly(2024, 7, 20), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("e402d7b9-5b3f-41c2-a5e1-4bb8d116b99c"), FullName = "Bui Khoa", Email = "khoa.bui@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 8, 1), LastActive = new DateOnly(2024, 8, 1), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("d239f5be-8855-4b2b-b4b6-2b3f0899f9a0"), FullName = "Vo Thi Hoa", Email = "hoa.vo@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 8, 15), LastActive = new DateOnly(2024, 8, 20), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("13aaf681-c57e-4f03-8adf-4c3ce98e5e5a"), FullName = "Pham Tien", Email = "tien.pham@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 9, 1), LastActive = new DateOnly(2024, 8, 20), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("cd415a34-e177-432d-b8c3-9820482c6b78"), FullName = "Nguyen Mai", Email = "mai.nguyen@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 9, 15), LastActive = new DateOnly(2024, 8, 20), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("b2c5b9b5-1f2c-4a27-86cc-9d16c3d5e28b"), FullName = "Dang Hung", Email = "hung.dang@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 10, 1), LastActive = new DateOnly(2024, 8, 20), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("f6888b37-51f1-49de-a35a-6f8b5a5298e1"), FullName = "Le Anh", Email = "anh.le@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 10, 15), LastActive = new DateOnly(2024, 8, 20), Status = UserStatus.Pending },
    new User { Id = Guid.Parse("d31f6eb8-9173-4520-9f15-132c05c9ef65"), FullName = "Tran My", Email = "my.tran@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 11, 1), LastActive = new DateOnly(2024, 8, 20), Status = UserStatus.Pending },

    // DEACTIVATED USERS
    new User { Id = Guid.Parse("02fc0f91-2e5b-4bb2-9db7-7cd1f4d2651e"), FullName = "Pham Quoc", Email = "quoc.pham@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 11, 15), LastActive = new DateOnly(2023, 12, 1), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("f361cb50-e53e-43d3-a930-204a6e3c9054"), FullName = "Nguyen Dao", Email = "dao.nguyen@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 12, 1), LastActive = new DateOnly(2023, 12, 20), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("3a8bbf65-0e38-4a4e-a82e-f90f2378ffef"), FullName = "Le Khanh", Email = "khanh.le@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 12, 15), LastActive = new DateOnly(2024, 1, 5), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("7a178e3d-0b59-4768-b10a-8b264cfbf3d1"), FullName = "Vo Bao", Email = "bao.vo@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 12, 20), LastActive = new DateOnly(2024, 1, 15), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("f0e3fd63-9eb8-4413-88d1-9c64ff88c42e"), FullName = "Bui Lan", Email = "lan.bui@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 12, 25), LastActive = new DateOnly(2024, 1, 20), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("83cd597c-dc83-42b0-9e1f-0531cbcb7d88"), FullName = "Dang Chi", Email = "chi.dang@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 12, 30), LastActive = new DateOnly(2024, 2, 1), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("f10e421c-51d6-47a8-9c14-064c0f0e3cf2"), FullName = "Mai Thanh", Email = "thanh.mai@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = adminRole.Id, JoinedDate = new DateOnly(2023, 12, 31), LastActive = new DateOnly(2024, 2, 10), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("a35bb9a5-4b5b-4c32-9d83-1b1cc3322cfb"), FullName = "Hoang Yen", Email = "yen.hoang@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = mentorRole.Id, JoinedDate = new DateOnly(2023, 12, 31), LastActive = new DateOnly(2024, 2, 15), Status = UserStatus.Deactivated },
    new User { Id = Guid.Parse("f08e49f7-9c1f-4c7b-9641-85c7bb50e1f7"), FullName = "Tran Duy", Email = "duy.tran@example.com", PasswordHash = PasswordHelper.HashPassword("password123"), RoleId = learnerRole.Id, JoinedDate = new DateOnly(2023, 12, 31), LastActive = new DateOnly(2024, 2, 20), Status = UserStatus.Deactivated }

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