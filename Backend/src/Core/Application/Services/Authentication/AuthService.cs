using System.Net;
using Application.Helpers;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Infrastructure.Services.Authorization;

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

    public async Task RegisterAsync(SignUpRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new ArgumentException("Password and confirm password do not match.");
        }
        var passwordHash = PasswordHelper.HashPassword(request.Password);
        var user = new User
        {
            Username = "",
            Email = request.Email,
            PasswordHash = passwordHash,
            RoleId = request.RoleId
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
    }

    public async Task<Result> LoginGithubAsync(OAuthSignInRequest request)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.GitHub);
        var accessToken = await oAuthService.GetAccessTokenAsync(request.Token);
        var userEmail = await oAuthService.GetUserEmailDataAsync(accessToken!);
        var token = await LoginOrRegisterAsync(userEmail!);

        return Result.Success(token, HttpStatusCode.OK);
    }

    public async Task<Result> LoginGoogleAsync(OAuthSignInRequest request)
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

        var newHashedPassword = PasswordHelper.HashPassword(request.NewPassword);
        user.PasswordHash = newHashedPassword;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

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
                Username = "",
                Email = email,
                RoleId = 3
            };
            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
        }
        user = await userRepository.GetUserByEmail(email);

        return jwtService.GenerateToken(user!);
    }

}