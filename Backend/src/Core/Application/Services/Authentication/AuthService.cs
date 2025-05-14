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
        var user = await userRepository.GetUserByUsername(request.Username);
        if (user == null)
        {
            return Result.Failure<string>("Null user", HttpStatusCode.NotFound);
        }

        var isVerified = PasswordHelper.VerifyPassword(request.Password, user!.PasswordHash);
        if (!isVerified)
        {
            return Result.Failure<string>("Invalid password", HttpStatusCode.Unauthorized);
        }

        var token = jwtService.GenerateToken(user);

        return Result.Success(token, HttpStatusCode.OK);
    }

    public async Task RegisterAsync(SignUpRequest request)
    {
        var passwordHash = PasswordHelper.HashPassword(request.Password);
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            RoleId = request.RoleId
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
    }

    public async Task<object?> LoginGithubAsync(string code)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.GitHub);
        var token = await oAuthService.GetAccessTokenAsync(code);
        var response = await oAuthService.GetUserEmailDataAsync(token);

        return response;
    }

    public async Task<object?> LoginGoogleAsync(string code)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.Google);
        var token = await oAuthService.GetAccessTokenAsync(code);
        var response = await oAuthService.GetUserEmailDataAsync(token);

        return response;
    }
}