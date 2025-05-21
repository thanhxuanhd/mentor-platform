using Application.Helpers;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using System.Net;

namespace Application.Services.Authentication;

public class AuthService(IUserRepository userRepository, IJwtService jwtService, IOAuthServiceFactory oAuthServiceFactory) : IAuthService
{
    public async Task<Result<string>> LoginAsync(SignInRequest request)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        if (user == null)
        {
            return Result.Failure<string>("Null user", HttpStatusCode.NotFound);
        }

        var isVerified = PasswordHelper.VerifyPassword(request.Password, user!.PasswordHash!);
        if (!isVerified)
        {
            return Result.Failure<string>("Invalid password", HttpStatusCode.Unauthorized);
        }

        var token = jwtService.GenerateToken(user);

        return Result.Success(token, HttpStatusCode.OK);
    }

    public async Task<Result<string>> RegisterAsync(SignUpRequest request)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        if (user != null)
        {
            return Result.Failure<string>("Email already existed!", HttpStatusCode.BadRequest);
        }
        var passwordHash = PasswordHelper.HashPassword(request.Password);
        var newUser = new User
        {
            FullName = "",
            PhoneNumber = "",
            Email = request.Email,
            PasswordHash = passwordHash,
            RoleId = request.RoleId,
            JoinedDate = DateOnly.FromDateTime(DateTime.Now)
        };

        await userRepository.AddAsync(newUser);
        await userRepository.SaveChangesAsync();
        newUser = await userRepository.GetUserByEmail(request.Email);

        var token = jwtService.GenerateToken(newUser!);

        return Result.Success(token, HttpStatusCode.OK);
    }

    public async Task<Result<string>> LoginGithubAsync(OAuthSignInRequest request)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.GitHub);
        var accessToken = await oAuthService.GetAccessTokenAsync(request.Token);
        var userEmail = await oAuthService.GetUserEmailDataAsync(accessToken!);
        var token = await LoginOrRegisterAsync(userEmail!);

        return Result.Success(token, HttpStatusCode.OK);
    }

    public async Task<Result<string>> LoginGoogleAsync(OAuthSignInRequest request)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.Google);
        var accessToken = await oAuthService.GetAccessTokenAsync(request.Token);
        var userEmail = await oAuthService.GetUserEmailDataAsync(accessToken!);
        var token = await LoginOrRegisterAsync(userEmail!);

        return Result.Success(token, HttpStatusCode.OK);
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        if (user == null)
        {
            return Result.Failure("User not found", HttpStatusCode.NotFound);
        }

        if (!PasswordHelper.VerifyPassword(request.OldPassword, user.PasswordHash!))
        {
            return Result.Failure("Old password is incorrect", HttpStatusCode.Unauthorized);
        }
        var newHashedPassword = PasswordHelper.HashPassword(request.NewPassword);
        user.PasswordHash = newHashedPassword;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        var token = jwtService.GenerateToken(user);

        return Result.Success("Password reset successful", HttpStatusCode.OK);
    }

    public async Task<Result<bool>> CheckEmailExistsAsync(string email)
    {
        var user = await userRepository.GetUserByEmail(email);
        bool exists = user is not null;
        return Result.Success(exists, HttpStatusCode.OK);
    }

    private async Task<string> LoginOrRegisterAsync(string email)
    {
        var user = await userRepository.GetUserByEmail(email);
        if (user == null)
        {
            user = new User
            {
                FullName = "",
                PhoneNumber = "",
                Email = email,
                RoleId = (int)UserRole.Learner,
                JoinedDate = DateOnly.FromDateTime(DateTime.Now)
            };
            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
        }
        user = await userRepository.GetUserByEmail(email);
        var token = jwtService.GenerateToken(user!);

        return token;
    }
}