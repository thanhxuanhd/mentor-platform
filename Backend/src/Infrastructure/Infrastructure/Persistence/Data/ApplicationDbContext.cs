using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseResource> CourseResources { get; set; }
    public DbSet<CourseTag> CourseTags { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Expertise> Expertises { get; set; }
    public DbSet<UserExpertise> UserExpertises { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<UserAvailability> UserAvailabilities { get; set; }
    public DbSet<TeachingApproach> TeachingApproaches { get; set; }
    public DbSet<UserCategory> UserCategories { get; set; }
    public DbSet<UserTeachingApproach> UserTeachingApproaches { get; set; }
    public DbSet<Schedules> Schedules { get; set; }
    public DbSet<MentorAvailableTimeSlot> MentorAvailableTimeSlots { get; set; }
    public DbSet<Sessions> Sessions { get; set; }
    public DbSet<MentorApplication> MentorApplications { get; set; }
    public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}