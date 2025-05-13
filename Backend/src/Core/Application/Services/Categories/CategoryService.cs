using Contract.Dtos.Categories.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using System.Net;

namespace Application.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<Result<GetCategoryResponse>> GetCategoriesAsync(int pageIndex, int pageSize, string keyword)
    {
        var categories = categoryRepository.GetAll();

        if (!string.IsNullOrEmpty(keyword))
        {
            categories = categories.Where(c => c.Name.Contains(keyword));
        }

        PaginatedList<Category> paginatedCategories = await categoryRepository.ToPaginatedListAsync<Category>(categories, pageSize, pageIndex);

        List<CategoryInfo> categoryInfos = new List<CategoryInfo>();

        foreach (var category in paginatedCategories.Items)
        {
            categoryInfos.Add(new CategoryInfo
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Courses = category.Courses.Count,
                Status = category.Status
            });
        }

        var response = new GetCategoryResponse
        {
            Categories = categoryInfos
        };

        return Result.Success(response, HttpStatusCode.OK);
    }
}

