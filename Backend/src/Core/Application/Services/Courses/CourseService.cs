using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Contract.Shared;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services.Courses;

public class Course : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }

    public Guid CategoryId { get; set; }
    // public Category Category { get; set; } = null!;

    public Guid MentorId { get; set; }
    // public User Mentor { get; set; }

    public List<CourseItem> Items { get; set; }
}

public class CourseItem : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public CourseMediaType MediaType { get; set; }
    public string WebAddress { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}

[JsonConverter(typeof(JsonStringEnumConverter<CourseStates>))]
public enum CourseStates
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

[JsonConverter(typeof(JsonStringEnumConverter<CourseMediaType>))]
public enum CourseMediaType
{
    Pdf = 0,
    Video = 1,
    ExternalLink = 2,
}

[JsonConverter(typeof(JsonStringEnumConverter<CourseDifficulty>))]
public enum CourseDifficulty
{
    Beginner = 0,
    Intermediate = 1,
    Advanced = 2,
}

public record DeleteCourseResponse
{
    public Guid Id { get; set; }
}

public record CourseCreateRequest
{
    [Length(1, 256)] public required string Title { get; init; }
    [Length(1, 256)] public required string Description { get; init; }
    [Length(1, 256)] public required string CategoryId { get; init; }
    public required DateTime DueDate { get; init; }
    public required List<string> Tags { get; init; }
}

public record CourseCreateResponse
{
}

public record CourseUpdateRequest
{
    [Length(1, 256)] public required string Title { get; init; }
    [Length(1, 256)] public required string Description { get; init; }
    [Length(1, 256)] public required string CategoryId { get; init; }
    public required DateTime DueDate { get; init; }
    public required List<string> Tags { get; init; }
}

public record CourseUpdateResponse
{
}

public record CourseListRequest
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public Guid CategoryId { get; init; }
    public Guid MentorId { get; init; }
    public CourseDifficulty Level { get; init; }
}

public record CourseListResponse : PaginatedList<CourseSummary>
{
}

// public record PaginationRequest
// {
//     [DefaultValue(10)] public int PageIndex { get; init; }
//     [DefaultValue(0)] public int PageSize { get; init; }
// }

public interface ICourseService
{
    public Task<CourseListResponse> GetAllAsync(CourseListRequest request);
    public Task<CourseCreateResponse> CreateAsync(CourseCreateRequest request);
    public Task<CourseUpdateResponse> UpdateAsync(CourseUpdateRequest request);
    public Task DeleteAsync(Guid id);
}

public class CourseService : ICourseService
{
}