using Application.Helpers;
using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System.Net;

namespace Application.Test;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IJwtService> _mockJwtService;
    private Mock<IOAuthServiceFactory> _mockOAuthServiceFactory;
    private Mock<IOAuthService> _mockOAuthService;
    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockOAuthServiceFactory = new Mock<IOAuthServiceFactory>();
        _mockOAuthService = new Mock<IOAuthService>();

        _mockOAuthServiceFactory.Setup(f => f.Create(It.IsAny<OAuthProvider>()))
            .Returns(_mockOAuthService.Object);

        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockJwtService.Object,
            _mockOAuthServiceFactory.Object);
    }

    private User CreateTestUser(string email, string? password = null, int roleId = (int)UserRole.Learner, UserStatus status = UserStatus.Active)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = password != null ? PasswordHelper.HashPassword(password) : null,
            RoleId = roleId,
            Status = status,
            JoinedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            FullName = "Test User",
            PhoneNumber = "1234567890"
        };
    }

    [Test]
    public async Task LoginAsync_UserNotFound_ReturnsFailureNotFound()
    {
        // Arrange
        var request = new SignInRequest("nonexistent@example.com", "password");
        _mockUserRepository.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync((User)null!);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Null user"));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task LoginAsync_InvalidPassword_ReturnsFailureUnauthorized()
    {
        // Arrange
        var request = new SignInRequest("test@example.com", "wrongpassword");
        var user = CreateTestUser(request.Email, "correctpassword");
        _mockUserRepository.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Invalid password"));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithAuthResponse()
    {
        // Arrange
        const string password = "correctpassword";
        var request = new SignInRequest("test@example.com", password);
        var user = CreateTestUser(request.Email, password, (int)UserRole.Admin, UserStatus.Active);
        const string expectedToken = "generated_jwt_token";

        _mockUserRepository.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(user);
        _mockJwtService.Setup(s => s.GenerateToken(user)).Returns(expectedToken);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Token, Is.EqualTo(expectedToken));
            Assert.That(result.Value.UserId, Is.EqualTo(user.Id));
            Assert.That(result.Value.UserStatus, Is.EqualTo(user.Status.ToString()));
        });
    }

    [Test]
    public async Task RegisterAsync_EmailAlreadyExists_ReturnsFailureBadRequest()
    {
        // Arrange
        var request = new SignUpRequest("Password123!", "Password123!", "test@example.com", (int)UserRole.Learner);
        var existingUser = CreateTestUser(request.Email);
        _mockUserRepository.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("User email already existed"));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Value, Is.Null);
        });

        _mockUserRepository.Verify(r => r.GetUserByEmail(request.Email), Times.Once());
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never());
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task RegisterAsync_ValidRequest_CreatesUserAndReturnsSuccess()
    {
        // Arrange
        var request = new SignUpRequest("Password123!", "Password123!", "newuser@example.com", (int)UserRole.Learner);
        User? existingUser = null;
        User? createdUser = null;
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            RoleId = request.RoleId,
            Status = UserStatus.Active,
            FullName = "",
            PhoneNumber = ""
        };
        _mockUserRepository.SetupSequence(r => r.GetUserByEmail(request.Email))
            .ReturnsAsync(existingUser)
            .ReturnsAsync(newUser);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => createdUser = u)
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        const string expectedToken = "jwt-token";
        _mockJwtService.Setup(j => j.GenerateToken(newUser)).Returns(expectedToken);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Token, Is.EqualTo(expectedToken));
            Assert.That(result.Value.UserId, Is.EqualTo(newUser.Id));
            Assert.That(result.Value.UserStatus, Is.EqualTo(newUser.Status.ToString()));
        });

        Assert.Multiple(() =>
        {
            Assert.That(createdUser, Is.Not.Null);
            Assert.That(createdUser!.Email, Is.EqualTo(request.Email));
            Assert.That(createdUser.RoleId, Is.EqualTo(request.RoleId));
            Assert.That(createdUser.FullName, Is.Empty);
            Assert.That(createdUser.PhoneNumber, Is.Empty);
            Assert.That(createdUser.PasswordHash, Is.Not.Null);
            Assert.That(PasswordHelper.VerifyPassword(request.Password, createdUser.PasswordHash!), Is.True);
        });

        _mockUserRepository.Verify(r => r.GetUserByEmail(request.Email), Times.Exactly(2));
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once());
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        _mockJwtService.Verify(j => j.GenerateToken(newUser), Times.Once());
    }

    [Test]
    public void RegisterAsync_RepositoryFailsToSave_ReturnsSuccessWithUpdatedUser()
    {
        // Arrange
        var request = new SignUpRequest("Password123!", "Password123!", "newuser@example.com", (int)UserRole.Learner);
        _mockUserRepository.Setup(r => r.GetUserByEmail(request.Email)).ReturnsAsync((User)null!);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("Database failure"));
        _mockJwtService.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("jwt-token");

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _authService.RegisterAsync(request), "Database failure");

        _mockUserRepository.Verify(r => r.GetUserByEmail(request.Email), Times.Once());
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once());
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        _mockJwtService.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never());
    }

    [Test]
    public async Task LoginGithubAsync_UserExists_ReturnsSuccessWithAuthResponse()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest("github_token");
        const string userEmail = "githubuser@example.com";
        const string accessToken = "github_access_token";
        var existingUser = CreateTestUser(userEmail, roleId: (int)UserRole.Learner, status: UserStatus.Active);
        const string expectedJwtToken = "jwt_for_github_user";

        _mockOAuthServiceFactory.Setup(f => f.Create(OAuthProvider.GitHub)).Returns(_mockOAuthService.Object);
        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token)).ReturnsAsync(accessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(accessToken)).ReturnsAsync(userEmail);
        _mockUserRepository.Setup(r => r.GetUserByEmail(userEmail)).ReturnsAsync(existingUser);
        _mockJwtService.Setup(s => s.GenerateToken(existingUser)).Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGithubAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Token, Is.EqualTo(expectedJwtToken));
            Assert.That(result.Value.UserId, Is.EqualTo(existingUser.Id));
            Assert.That(result.Value.UserStatus, Is.EqualTo(existingUser.Status.ToString()));
        });
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task LoginGithubAsync_NewUser_RegistersAndReturnsSuccessWithAuthResponse()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest("github_new_user_token");
        const string userEmail = "newgithubuser@example.com";
        const string accessToken = "github_new_access_token";
        var expectedJwtToken = "jwt_for_new_github_user";
        User? addedUser = null;

        _mockOAuthServiceFactory.Setup(f => f.Create(OAuthProvider.GitHub)).Returns(_mockOAuthService.Object);
        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token)).ReturnsAsync(accessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(accessToken)).ReturnsAsync(userEmail);
        _mockUserRepository.SetupSequence(r => r.GetUserByEmail(userEmail))
            .ReturnsAsync((User)null!)
            .ReturnsAsync(() => addedUser);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u =>
            {
                u.Id = Guid.NewGuid();
                u.Status = UserStatus.Active;
                addedUser = u;
            })
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockJwtService.Setup(s => s.GenerateToken(It.Is<User>(u => u.Email == userEmail)))
                       .Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGithubAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Token, Is.EqualTo(expectedJwtToken));
            Assert.That(addedUser, Is.Not.Null);
            Assert.That(addedUser!.Email, Is.EqualTo(userEmail));
            Assert.That(addedUser.RoleId, Is.EqualTo((int)UserRole.Learner));
            Assert.That(result.Value.UserStatus, Is.EqualTo(UserStatus.Active.ToString()));
        });
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == userEmail && u.RoleId == (int)UserRole.Learner)), Times.Once);
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task LoginGoogleAsync_UserExists_ReturnsSuccessWithAuthResponse()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest("google_token");
        const string userEmail = "googleuser@example.com";
        const string accessToken = "google_access_token";
        var existingUser = CreateTestUser(userEmail, roleId: (int)UserRole.Admin, status: UserStatus.Pending);
        const string expectedJwtToken = "jwt_for_google_user";

        _mockOAuthServiceFactory.Setup(f => f.Create(OAuthProvider.Google)).Returns(_mockOAuthService.Object);
        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token)).ReturnsAsync(accessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(accessToken)).ReturnsAsync(userEmail);
        _mockUserRepository.Setup(r => r.GetUserByEmail(userEmail)).ReturnsAsync(existingUser);
        _mockJwtService.Setup(s => s.GenerateToken(existingUser)).Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGoogleAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Token, Is.EqualTo(expectedJwtToken));
            Assert.That(result.Value.UserId, Is.EqualTo(existingUser.Id));
            Assert.That(result.Value.UserStatus, Is.EqualTo(existingUser.Status.ToString()));
        });
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task LoginGoogleAsync_NewUser_RegistersAndReturnsSuccessWithAuthResponse()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest("google_new_user_token");
        const string userEmail = "newgoogleuser@example.com";
        const string accessToken = "google_new_access_token";
        const string expectedJwtToken = "jwt_for_new_google_user";
        User? addedUser = null;

        _mockOAuthServiceFactory.Setup(f => f.Create(OAuthProvider.Google)).Returns(_mockOAuthService.Object);
        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token)).ReturnsAsync(accessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(accessToken)).ReturnsAsync(userEmail);
        _mockUserRepository.SetupSequence(r => r.GetUserByEmail(userEmail))
           .ReturnsAsync((User)null!)
           .ReturnsAsync(() => addedUser);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
           .Callback<User>(u =>
           {
               u.Id = Guid.NewGuid();
               u.Status = UserStatus.Active;
               addedUser = u;
           })
           .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockJwtService.Setup(s => s.GenerateToken(It.Is<User>(u => u.Email == userEmail)))
                      .Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGoogleAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Token, Is.EqualTo(expectedJwtToken));
            Assert.That(addedUser, Is.Not.Null);
            Assert.That(addedUser!.Email, Is.EqualTo(userEmail));
            Assert.That(addedUser.RoleId, Is.EqualTo((int)UserRole.Learner));
            Assert.That(result.Value.UserStatus, Is.EqualTo(UserStatus.Active.ToString()));
        });
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == userEmail && u.RoleId == (int)UserRole.Learner)), Times.Once);
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ResetPasswordAsync_IncorrectOldPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@example.com", "wrongPassword123", "newPassword123");
        var user = CreateTestUser(request.Email, "oldPassword123");
        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ResetPasswordAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Old password is incorrect"));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        });

        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task ResetPasswordAsync_ValidCredentials_UpdatesPasswordAndReturnsSuccess()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@example.com", "oldPassword123", "newPassword123");
        var user = CreateTestUser(request.Email, "oldPassword123");
        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(repo => repo.Update(user))
            .Callback(() => user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword));
        _mockUserRepository.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _authService.ResetPasswordAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(PasswordHelper.VerifyPassword(request.NewPassword, user.PasswordHash!), Is.True, "New password should be hashed and verified correctly.");
        });

        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }


    [Test]
    public async Task ResetPasswordAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new ResetPasswordRequest(Email: "nonexistent@example.com", OldPassword: "olePassword123", NewPassword: "newPassword123");

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _authService.ResetPasswordAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("User not found"));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });

        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CheckEmailExistsAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        const string email = "exists@example.com";
        var user = new User { Id = Guid.NewGuid(), Email = email };

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.CheckEmailExistsAsync(email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True, "Result value should be true as email exists.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
    }

    [Test]
    public async Task CheckEmailExistsAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string email = "nonexistent@example.com";

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _authService.CheckEmailExistsAsync(email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.False, "Result value should be false as email does not exist.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
    }
}