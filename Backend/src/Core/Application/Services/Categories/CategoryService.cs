using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using System.Net;

namespace Application.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<Result<PaginatedList<GetCategoryResponse>>> GetCategoriesAsync(int pageIndex, int pageSize, string keyword)
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            return Result.Failure<PaginatedList<GetCategoryResponse>>("Page index and page size must be greater than or equal to 0", HttpStatusCode.BadRequest);
        }

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
        if (pageIndex <= 0 || pageSize <= 0)
        {
            return Result.Failure<PaginatedList<FilterCourseByCategoryResponse>>("Page index and page size must be greater than or equal to 0", HttpStatusCode.BadRequest);
        }

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

    public async Task<Result<GetCategoryResponse>> CreateCategoryAsync(CategoryRequest request)
    {
        if (await categoryRepository.ExistByNameAsync(request.Name))
        {
            return Result.Failure<GetCategoryResponse>("Already have this category", HttpStatusCode.BadRequest);
        }
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status
        };
        await categoryRepository.AddAsync(category);
        var result = await categoryRepository.SaveChangesAsync();
        return Result.Success(new GetCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Courses = 0,
            Status = category.Status
        }, HttpStatusCode.Created);
    }

    public async Task<Result<bool>> EditCategoryAsync(Guid categoryId, CategoryRequest request)
    {

        if (await categoryRepository.ExistByNameExcludeAsync(categoryId, request.Name))
        {
            return Result.Failure<bool>("Already have this category", HttpStatusCode.BadRequest);
        }
        var category = await categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            return Result.Failure<bool>("Categories is not found or is deleted", HttpStatusCode.NotFound);
        }
        category.Name = request.Name;
        category.Description = request.Description;
        category.Status = request.Status;
        categoryRepository.Update(category);
        var result = await categoryRepository.SaveChangesAsync();
        return Result.Success(true, HttpStatusCode.OK);
    }
}

