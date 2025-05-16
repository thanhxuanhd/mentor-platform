using System.Net;
using Contract.Dtos.Categories.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<Result<PaginatedList<GetCategoryResponse>>> GetCategoriesAsync(int pageIndex, int pageSize,
        string keyword)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return Result.Failure<PaginatedList<GetCategoryResponse>>(
                "Page index and page size must be greater than or equal to 0", HttpStatusCode.BadRequest);

        var categories = categoryRepository.GetAll();

        if (!string.IsNullOrEmpty(keyword)) categories = categories.Where(c => c.Name.Contains(keyword));

        var categoryInfos = categories.Select(c => new GetCategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Courses = c.Courses.Count(),
            Status = c.Status
        });

        var paginatedCategories = await categoryRepository.ToPaginatedListAsync(categoryInfos, pageSize, pageIndex);

        return Result.Success(paginatedCategories, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<FilterCourseByCategoryResponse>>> FilterCourseByCategoryAsync(Guid id,
        int pageIndex, int pageSize)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return Result.Failure<PaginatedList<FilterCourseByCategoryResponse>>(
                "Page index and page size must be greater than or equal to 0", HttpStatusCode.BadRequest);

        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
            return Result.Failure<PaginatedList<FilterCourseByCategoryResponse>>("Category not found",
                HttpStatusCode.NotFound);

        var courses = categoryRepository.FilterCourseByCategory(id);

        var courseInfos = courses.Select(c => new FilterCourseByCategoryResponse
        {
            Id = c.Id,
            Title = c.Title,
            CategoryName = category.Name,
            Status = c.Status.ToString(),
            Description = c.Description,
            Difficulty = c.Difficulty.ToString(),
            DueDate = c.DueDate,
            Tags = c.CourseTags.Select(ct => ct.Tag.Name).ToList()
        });

        var paginatedCourses = await categoryRepository.ToPaginatedListAsync(courseInfos, pageSize, pageIndex);

        return Result.Success(paginatedCourses, HttpStatusCode.OK);
    }
}