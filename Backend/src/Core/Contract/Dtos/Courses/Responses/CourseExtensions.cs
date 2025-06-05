using Contract.Dtos.CourseResources.Responses;
using Domain.Entities;

namespace Contract.Dtos.Courses.Responses;

public static class CourseExtensions
{
    public static CourseSummaryResponse ToCourseSummaryResponse(this Course course)
    {
        return new CourseSummaryResponse
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
            Resources = course.Resources.Select(ToCourseResourceResponse).ToList(),
            Tags = course.Tags.Select(t => t.Name).ToList(),
            Status = course.Status
        };
    }

    public static CourseResourceResponse ToCourseResourceResponse(this CourseResource courseResource)
    {
        return new CourseResourceResponse
        {
            Id = courseResource.Id,
            Description = courseResource.Description,
            Title = courseResource.Title,
            ResourceType = courseResource.ResourceType,
            ResourceUrl = courseResource.ResourceUrl,
            CourseTitle = courseResource.Course.Title,
        };
    }
}