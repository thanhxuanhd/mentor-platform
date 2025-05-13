using Application.Helpers;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;

namespace Application.Services.Authentication;

public class AuthService(IUserRepository userRepository, IJwtService jwtService) : IAuthService
{
    public async Task<Result<string>> LoginAsync(SignInRequest request)
    {
        var user = await userRepository.GetUserByUsername(request.Username);
        if (user == null)
        {
            return Result.Failure<string>("Null user");
        }

        var isVerified = PasswordHelper.VerifyPassword(request.Password, user!.PasswordHash);
        if (!isVerified)
        {
            return Result.Failure<string>("Invalid password");
        }

        var token = jwtService.GenerateToken(user);

        return Result.Success(token);
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
}