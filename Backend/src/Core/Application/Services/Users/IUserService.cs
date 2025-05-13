using Contract.Dtos.Users.Responses;
using Contract.Shared;
using Domain.Entities;

namespace Application.Services.Users;

public interface IUserService
{
    Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id);
    Task<PaginatedList<User>> GetList();
}