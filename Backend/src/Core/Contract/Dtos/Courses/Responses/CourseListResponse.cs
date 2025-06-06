using System;
using Contract.Shared;

namespace Contract.Dtos.Courses.Responses;

public class CourseListResponse(List<CourseSummary> items, int count, int pageIndex, int pageSize)
    : PaginatedList<CourseSummary>(items, count, pageIndex, pageSize);