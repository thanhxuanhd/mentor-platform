using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Contract.Dtos.Courses.Requests;

public record CourseListRequest
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? MentorId { get; init; }
}