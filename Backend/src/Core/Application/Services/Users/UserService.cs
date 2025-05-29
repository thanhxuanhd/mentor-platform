using Application.Helpers;
using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services.Users;

public class UserService(IUserRepository userRepository, IEmailService emailService, IWebHostEnvironment env, ILogger<UserService> logger) : IUserService
{
    public async Task<Result<GetUserResponse>> GetUserByEmailAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("User not found", HttpStatusCode.NotFound);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }

    public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>($"User with id {id} not found.", HttpStatusCode.NotFound);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<GetUserResponse>>> FilterUserAsync(UserFilterPagedRequest request)
    {
        var users = userRepository.GetAll();

        if (!string.IsNullOrEmpty(request.FullName))
        {
            users = users.Where(user => user.FullName.ToLower().Contains(request.FullName.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.RoleName))
        {
            users = users.Where(user => user.Role.Name.ToString().Equals(request.RoleName));
        }

        var usersResponse = users.Select(u => new GetUserResponse()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.Name.ToString(),
            Status = u.Status.ToString(),
            JoinedDate = u.JoinedDate,
            LastActive = u.LastActive
        });

        PaginatedList<GetUserResponse> paginatedUsers = await userRepository.ToPaginatedListAsync(usersResponse, request.PageSize, request.PageIndex);

        return Result.Success(paginatedUsers, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> EditUserAsync(Guid id, EditUserRequest request)
    {
        if (await userRepository.ExistByEmailExcludeAsync(id, request.Email))
        {
            return Result.Failure<bool>($"Email {request.Email} already exists.", HttpStatusCode.Conflict);
        }

        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result.Failure<bool>($"User with id {id} not found.", HttpStatusCode.NotFound);
        }

        if (!Enum.TryParse(typeof(UserRole), request.Role, out var roleEnum))
        {
            return Result.Failure<bool>($"Invalid role: {request.Role}", HttpStatusCode.BadRequest);
        }
        else
        {
            user.RoleId = (int)roleEnum;
        }
        user.FullName = request.FullName;
        user.Email = request.Email;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> ChangeUserStatusAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return Result.Failure<bool>($"User with id {userId} not found.", HttpStatusCode.NotFound);
        }

        user.Status = user.Status == UserStatus.Active ? UserStatus.Deactivated : UserStatus.Active;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result> EditUserDetailAsync(Guid userId, EditUserProfileRequest request)
    {
        var user = await userRepository.GetUserDetailAsync(userId);
        if (user == null)
        {
            return Result.Failure($"User with ID {userId} not found.", HttpStatusCode.NotFound);
        }
        if (request.AvailabilityIds is not null &&
            !await userRepository.CheckEntityListExist<Availability, Guid>(request.AvailabilityIds))
        {
            return Result.Failure("Invalid Availability IDs", HttpStatusCode.BadRequest);
        }
        if (request.ExpertiseIds is not null &&
            !await userRepository.CheckEntityListExist<Expertise, Guid>(request.ExpertiseIds))
        {
            return Result.Failure("Invalid Expertise IDs", HttpStatusCode.BadRequest);
        }
        if (request.TeachingApproachIds is not null &&
            !await userRepository.CheckEntityListExist<TeachingApproach, Guid>(request.TeachingApproachIds))
        {
            return Result.Failure("Invalid Teaching Approach IDs", HttpStatusCode.BadRequest);
        }
        if (request.CategoryIds is not null &&
            !await userRepository.CheckEntityListExist<Category, Guid>(request.CategoryIds))
        {
            return Result.Failure("Invalid Category IDs", HttpStatusCode.BadRequest);
        }

        if (request.AvailabilityIds != null)
        {
            user.UserAvailabilities.Clear();
            user.UserAvailabilities = request.AvailabilityIds
                .Select(id => new UserAvailability { UserId = user.Id, AvailabilityId = id })
                .ToList();
        }
        if (request.ExpertiseIds != null)
        {
            user.UserExpertises.Clear();
            user.UserExpertises = request.ExpertiseIds
                .Select(id => new UserExpertise { UserId = user.Id, ExpertiseId = id })
                .ToList();
        }
        if (request.CategoryIds != null)
        {
            user.UserCategories.Clear();
            user.UserCategories = request.CategoryIds
                .Select(id => new UserCategory { UserId = user.Id, CategoryId = id })
                .ToList();
        }
        if (request.TeachingApproachIds != null)
        {
            user.UserTeachingApproaches.Clear();
            user.UserTeachingApproaches = request.TeachingApproachIds
                .Select(id => new UserTeachingApproach { UserId = user.Id, TeachingApproachId = id })
                .ToList();
        }

        request.ToUser(user);
        user.Status = UserStatus.Active;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return Result.Success(HttpStatusCode.OK);
    }

    public async Task<Result> ForgotPasswordRequest(string email)
    {
        var user = await userRepository.GetByEmailAsync(email, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("User not found", HttpStatusCode.NotFound);
        }
        var newPassword = GenerateRandomPassword(10);
        var newHashedPassword = PasswordHelper.HashPassword(newPassword);
        user.PasswordHash = newHashedPassword;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        var subject = EmailConstants.SUBJECT_RESET_PASSWORD;
        var body = EmailConstants.BodyResetPasswordEmail(user.Email, newPassword);

        var isSuccess = await emailService.SendEmailAsync(user.Email, subject, body);
        if (!isSuccess)
        {
            return Result.Failure("Failed to send email", HttpStatusCode.InternalServerError);
        }

        return Result.Success(new
        {
            message = "Password reset successful. Please check your email.",
            newPassword = newPassword
        }, HttpStatusCode.OK);
    }

    public async Task<Result<GetUserDetailResponse>> GetUserDetailAsync(Guid userId)
    {
        var user = await userRepository.GetUserDetailAsync(userId);
        if (user == null)
        {
            return Result.Failure<GetUserDetailResponse>("User not found", HttpStatusCode.NotFound);
        }

        return Result.Success(user.ToGetUserDetailResponse(), HttpStatusCode.OK);
    }

    private string GenerateRandomPassword(int length)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<Result<string>> UploadAvatarAsync(Guid userId, HttpRequest request, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Failure<string>("File not selected", HttpStatusCode.BadRequest);
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure<string>($"User with ID {userId} not found", HttpStatusCode.NotFound);
        }

        var fileContentType = file.ContentType;

        if (!FileConstants.IMAGE_CONTENT_TYPES.Contains(fileContentType))
        {
            return Result.Failure<string>("File content type is not allowed.", HttpStatusCode.BadRequest);
        }

        if (file.Length > FileConstants.MAX_IMAGE_SIZE)
        {
            return Result.Failure<string>("File size must not exceed 1MB.", HttpStatusCode.BadRequest);
        }

        var path = Directory.GetCurrentDirectory();
        logger.LogInformation($"RootPath: {env.WebRootPath}");
        if (!Directory.Exists(Path.Combine(path, env.WebRootPath)))
        {
            Directory.CreateDirectory(Path.Combine(path, env.WebRootPath));
        }
        var imagesPath = Path.Combine(path, env.WebRootPath, "images");

        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }

        try
        {
            var userIdStr = userId.ToString();

            var existingFile = Directory.GetFiles(imagesPath)
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f)
                            .Split("_")[0].Equals(userIdStr, StringComparison.OrdinalIgnoreCase));

            if (existingFile != null)
            {
                File.Delete(existingFile);
            }

            long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var fileName = $"{userIdStr}_{epoch}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(imagesPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream); var baseUrl = $"{request?.Scheme}://{request?.Host}";

            var fileUrl = $"{baseUrl}/images/{fileName}";

            return Result.Success(fileUrl, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Failed to save file: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public Result<bool> RemoveAvatar(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Result.Failure<bool>("Image URL is required.", HttpStatusCode.BadRequest);
        }

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Result.Failure<bool>("Invalid image URL format.", HttpStatusCode.BadRequest);
        }

        try
        {
            var fileName = Path.GetFileName(uri.LocalPath);
            var imagesPath = Path.Combine(env.WebRootPath, "images");
            var filePath = Path.Combine(imagesPath, fileName);

            if (!File.Exists(filePath))
            {
                return Result.Failure<bool>("Avatar file not found.", HttpStatusCode.NotFound);
            }

            File.Delete(filePath);

            return Result.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Failed to remove avatar: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}
