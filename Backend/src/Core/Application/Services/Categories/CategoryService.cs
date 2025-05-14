using Contract.Dtos.Categories.Responses;
using Contract.Repositories;
using Contract.Shared;
using System.Net;

namespace Application.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<Result<PaginatedList<GetCategoryResponse>>> GetCategoriesAsync(int pageIndex, int pageSize, string keyword)
    {
        var categories = categoryRepository.GetAll();

        if (!string.IsNullOrEmpty(keyword))
        {
            categories = categories.Where(c => c.Name.Contains(keyword));
        }

        var categoryInfos = categories.Select(c => new GetCategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Courses = c.Courses.Count(),
            Status = c.Status
        }); 

        PaginatedList<GetCategoryResponse> paginatedCategories = await categoryRepository.ToPaginatedListAsync<GetCategoryResponse>(categoryInfos, pageSize, pageIndex);

        return Result.Success(paginatedCategories, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<FilterCourseByCategoryResponse>>> FilterCourseByCategoryAsync(Guid id, int pageIndex, int pageSize)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return Result.Failure<PaginatedList<FilterCourseByCategoryResponse>>("Category not found", HttpStatusCode.NotFound);
        }

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

        PaginatedList<FilterCourseByCategoryResponse> paginatedCourses = await categoryRepository.ToPaginatedListAsync<FilterCourseByCategoryResponse>(courseInfos, pageSize, pageIndex);

        return Result.Success(paginatedCourses, HttpStatusCode.OK);
    }
}

