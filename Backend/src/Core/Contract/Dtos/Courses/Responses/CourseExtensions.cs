using Domain.Entities;

namespace Contract.Dtos.Courses.Responses;

public static class CourseExtensions
{
    public static CourseSummary ToCourseSummary(this Course course)
    {
        return new CourseSummary
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            CategoryName = course.Category.Name,
            MentorId = course.MentorId,
            MentorName = course.Mentor.FullName,
            Difficulty = course.Difficulty,
            DueDate = course.DueDate,
            Items = course.Items.Select(ToCourseItemDto).ToList(),
            Tags = course.Tags.Select(t => t.Name).ToList(),
            Status = course.Status
        };
    }

    public static CourseItemDto ToCourseItemDto(this CourseItem courseItem)
    {
        return new CourseItemDto
        {
            Id = courseItem.Id,
            Description = courseItem.Description,
            Title = courseItem.Title,
            MediaType = courseItem.MediaType,
            WebAddress = courseItem.WebAddress
        };
    }
}