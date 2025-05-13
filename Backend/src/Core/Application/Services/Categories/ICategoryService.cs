
using Contract.Dtos.Categories.Responses;
using Contract.Shared;

namespace Application.Services.Categories
{
    public interface ICategoryService
    {
        Task<Result<GetCategoryResponse>> GetCategoriesAsync(int pageIndex, int pageSize, string keyword);
    }
}