using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Shared;

namespace Application.Services.Categories;

public interface ICategoryService
{
    Task<Result<GetCategoryResponse>> GetCategoryByIdAsync(Guid id);
    Task<Result<PaginatedList<GetCategoryResponse>>> GetCategoriesAsync(FilterCategoryRequest request);
    Task<Result<List<FilterCourseByCategoryResponse>>> FilterCourseByCategoryAsync(Guid id);
    Task<Result<GetCategoryResponse>> CreateCategoryAsync(CategoryRequest request);
    Task<Result<bool>> EditCategoryAsync(Guid categoryId, CategoryRequest request);
    Task<Result<bool>> DeleteCategoryAsync(Guid categoryId);
}