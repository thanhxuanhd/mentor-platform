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
            Courses = c.Courses.Count,
            Status = c.Status
        }); 

        PaginatedList<GetCategoryResponse> paginatedCategories = await categoryRepository.ToPaginatedListAsync<GetCategoryResponse>(categoryInfos, pageSize, pageIndex);

        return Result.Success(paginatedCategories, HttpStatusCode.OK);
    }
}

