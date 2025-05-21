using Application.Helpers;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using System.Net;
using Contract.Dtos.Authentication.Responses;

namespace Application.Services.Authentication;

public class AuthService(IUserRepository userRepository, IJwtService jwtService, IOAuthServiceFactory oAuthServiceFactory) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(SignInRequest request)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        if (user == null)
        {
            return Result.Failure<AuthResponse>("Null user", HttpStatusCode.NotFound);
        }

        var isVerified = PasswordHelper.VerifyPassword(request.Password, user!.PasswordHash!);
        if (!isVerified)
        {
            return Result.Failure<AuthResponse>("Invalid password", HttpStatusCode.Unauthorized);
        }

        var signInResponse = ToSignInResponse(user);

        return Result.Success(signInResponse, HttpStatusCode.OK);
    }

    public async Task<Result<AuthResponse>> RegisterAsync(SignUpRequest request)
    {
        var user = await userRepository.GetUserByEmail(request.Email);
        if (user != null)
        {
            return Result.Failure<AuthResponse>("User email already existed", HttpStatusCode.BadRequest);
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

        return Result.Success(ToSignInResponse(newUser!), HttpStatusCode.OK);
    }

    public async Task<Result<AuthResponse>> LoginGithubAsync(OAuthSignInRequest request)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.GitHub);
        var accessToken = await oAuthService.GetAccessTokenAsync(request.Token);
        var userEmail = await oAuthService.GetUserEmailDataAsync(accessToken!);
        var authResponse = await LoginOrRegisterAsync(userEmail!);

        return Result.Success(authResponse, HttpStatusCode.OK);
    }

    public async Task<Result<AuthResponse>> LoginGoogleAsync(OAuthSignInRequest request)
    {
        var oAuthService = oAuthServiceFactory.Create(OAuthProvider.Google);
        var accessToken = await oAuthService.GetAccessTokenAsync(request.Token);
        var userEmail = await oAuthService.GetUserEmailDataAsync(accessToken!);
        var authResponse = await LoginOrRegisterAsync(userEmail!);

        return Result.Success(authResponse, HttpStatusCode.OK);
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

        return Result.Success("Password reset successful", HttpStatusCode.OK);
    }

    public async Task<Result<bool>> CheckEmailExistsAsync(string email)
    {
        var user = await userRepository.GetUserByEmail(email);
        bool exists = user is not null;
        return Result.Success(exists, HttpStatusCode.OK);
    }

    private async Task<AuthResponse> LoginOrRegisterAsync(string email)
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
            };
            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
        }
        user = await userRepository.GetUserByEmail(email);

        return ToSignInResponse(user!);
    }

    private AuthResponse ToSignInResponse(User user)
    {
        var token = jwtService.GenerateToken(user);
        var signInResponse = new AuthResponse(
            Token: token,
            UserId: user.Id,
            UserStatus: user.Status.ToString()
        );

        return signInResponse;
    }
}