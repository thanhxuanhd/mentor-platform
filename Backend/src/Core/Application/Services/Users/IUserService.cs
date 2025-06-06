using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Users;

public interface IUserService
{
    Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id);
    Task<Result<PaginatedList<GetUserResponse>>> FilterUserAsync(UserFilterPagedRequest request);
    Task<Result<bool>> EditUserAsync(Guid id, EditUserRequest request);
    Task<Result<bool>> ChangeUserStatusAsync(Guid userId);
    Task<Result<GetUserResponse>> GetUserByEmailAsync(string email);
    Task<Result> EditUserDetailAsync(Guid userId, EditUserProfileRequest request);
    Task<Result> ForgotPasswordRequest(string email);
    Task<Result<GetUserDetailResponse>> GetUserDetailAsync(Guid userId);
    Task<Result<string>> UploadAvatarAsync(Guid userId, HttpRequest request, IFormFile file);
    Result<bool> RemoveAvatar(string imageUrl);
    Task<Result<string>> UploadDocumentAsync(Guid userId, HttpRequest request, IFormFile file);
    Task<Result<bool>> RemoveDocumentAsync(Guid userId, string documentUrl);
}