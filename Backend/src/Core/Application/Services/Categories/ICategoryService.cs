
using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Shared;

namespace Application.Services.Categories;

public interface ICategoryService
{
    Task<Result<PaginatedList<GetCategoryResponse>>> GetCategoriesAsync(int pageIndex, int pageSize, string keyword);
    Task<Result<PaginatedList<FilterCourseByCategoryResponse>>> FilterCourseByCategoryAsync(Guid id, int pageIndex, int pageSize);
    Task<Result<GetCategoryResponse>> CreateCategoryAsync(CategoryRequest request);
    Task<Result<bool>> EditCategoryAsync(Guid categoryId, CategoryRequest request);
}