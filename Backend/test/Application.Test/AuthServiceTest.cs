using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services.Authorization;
using Moq;
using System.Net;
using Application.Helpers;

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
            _mockOAuthServiceFactory.Object
        );
    }

    [Test]
    public async Task LoginGithubAsync_UserExists_ReturnsSuccessWithToken()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest(Token: "github_oauth_token");
        const string githubAccessToken = "github_access_token";
        const string userEmail = "githubuser@example.com";
        const string expectedJwtToken = "jwt_token_for_github_user";
        var existingUser = new User { Id = Guid.NewGuid(), Email = userEmail, RoleId = (int)UserRole.Learner };

        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token))
            .ReturnsAsync(githubAccessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(githubAccessToken))
            .ReturnsAsync(userEmail);
        _mockUserRepository.Setup(repo => repo.GetUserByEmail(userEmail))
            .ReturnsAsync(existingUser);
        _mockJwtService.Setup(service => service.GenerateToken(existingUser))
            .Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGithubAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "Result should be successful.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be OK.");
        });

        _mockOAuthServiceFactory.Verify(f => f.Create(OAuthProvider.GitHub), Times.Once);
        _mockOAuthService.Verify(s => s.GetAccessTokenAsync(oauthRequest.Token), Times.Once);
        _mockOAuthService.Verify(s => s.GetUserEmailDataAsync(githubAccessToken), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(userEmail), Times.Exactly(2));
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockJwtService.Verify(service => service.GenerateToken(existingUser), Times.Once);
    }

    [Test]
    public async Task LoginGithubAsync_NewUser_RegistersAndReturnsSuccessWithToken()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest(Token: "github_oauth_token_new");
        const string githubAccessToken = "github_access_token_new";
        const string userEmail = "newgithubuser@example.com";
        const string expectedJwtToken = "jwt_token_for_new_github_user";
        User? capturedUserForAdd = null;
        User? capturedUserForToken = null;


        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token))
            .ReturnsAsync(githubAccessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(githubAccessToken))
            .ReturnsAsync(userEmail);

        _mockUserRepository.SetupSequence(repo => repo.GetUserByEmail(userEmail))
            .ReturnsAsync((User)null!)
            .ReturnsAsync(() => capturedUserForAdd);

        _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Callback<User>(u =>
            {
                capturedUserForAdd = u;
                capturedUserForAdd.Id = Guid.NewGuid();
            })
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockJwtService.Setup(service => service.GenerateToken(It.Is<User>(u => u.Email == userEmail)))
            .Callback<User>(u => capturedUserForToken = u)
            .Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGithubAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "Result should be successful.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be OK.");

            Assert.That(capturedUserForAdd, Is.Not.Null, "A user should have been captured for AddAsync.");
            Assert.That(capturedUserForAdd?.Email, Is.EqualTo(userEmail));
            Assert.That(capturedUserForAdd?.FullName, Is.EqualTo(""));
            Assert.That(capturedUserForAdd?.RoleId, Is.EqualTo((int)UserRole.Learner));
            Assert.That(capturedUserForAdd?.PasswordHash,
                Is.Null.Or.Empty); // OAuth users might not have a password hash initially

            Assert.That(capturedUserForToken, Is.Not.Null, "A user should have been passed to GenerateToken.");
            Assert.That(capturedUserForToken?.Email, Is.EqualTo(userEmail));
        });

        _mockOAuthServiceFactory.Verify(f => f.Create(OAuthProvider.GitHub), Times.Once);
        _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u =>
            u.Email == userEmail &&
            u.FullName == "" &&
            u.RoleId == (int)UserRole.Learner
        )), Times.Once);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(userEmail), Times.Exactly(2));
        _mockJwtService.Verify(service => service.GenerateToken(It.Is<User>(u => u.Email == userEmail)), Times.Once);
    }

    [Test]
    public async Task LoginGoogleAsync_UserExists_ReturnsSuccessWithToken()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest(Token: "google_oauth_token");
        const string googleAccessToken = "google_access_token";
        const string userEmail = "googleuser@example.com";
        const string expectedJwtToken = "jwt_token_for_google_user";
        var existingUser = new User { Id = Guid.NewGuid(), Email = userEmail, RoleId = (int)UserRole.Learner };

        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token))
            .ReturnsAsync(googleAccessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(googleAccessToken))
            .ReturnsAsync(userEmail);
        _mockUserRepository.Setup(repo => repo.GetUserByEmail(userEmail))
            .ReturnsAsync(existingUser);
        _mockJwtService.Setup(service => service.GenerateToken(existingUser))
            .Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGoogleAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });

        _mockOAuthServiceFactory.Verify(f => f.Create(OAuthProvider.Google), Times.Once);
        _mockOAuthService.Verify(s => s.GetAccessTokenAsync(oauthRequest.Token), Times.Once);
        _mockOAuthService.Verify(s => s.GetUserEmailDataAsync(googleAccessToken), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(userEmail), Times.Exactly(2));
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        _mockJwtService.Verify(service => service.GenerateToken(existingUser), Times.Once);
    }


    [Test]
    public async Task LoginGoogleAsync_NewUser_RegistersAndReturnsSuccessWithToken()
    {
        // Arrange
        var oauthRequest = new OAuthSignInRequest(Token: "google_oauth_token_new");
        const string googleAccessToken = "google_access_token_new";
        const string userEmail = "newgoogleuser@example.com";
        const string expectedJwtToken = "jwt_token_for_new_google_user";
        User? capturedUserForAdd = null;
        User? capturedUserForToken = null;


        _mockOAuthService.Setup(s => s.GetAccessTokenAsync(oauthRequest.Token))
            .ReturnsAsync(googleAccessToken);
        _mockOAuthService.Setup(s => s.GetUserEmailDataAsync(googleAccessToken))
            .ReturnsAsync(userEmail);
        _mockUserRepository.SetupSequence(repo => repo.GetUserByEmail(userEmail))
            .ReturnsAsync((User)null!)
            .ReturnsAsync(() => capturedUserForAdd);

        _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Callback<User>(u =>
            {
                capturedUserForAdd = u;
                capturedUserForAdd.Id = Guid.NewGuid();
            })
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockJwtService.Setup(service => service.GenerateToken(It.Is<User>(u => u.Email == userEmail)))
            .Callback<User>(u => capturedUserForToken = u)
            .Returns(expectedJwtToken);

        // Act
        var result = await _authService.LoginGoogleAsync(oauthRequest);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(capturedUserForAdd, Is.Not.Null);
            Assert.That(capturedUserForAdd?.Email, Is.EqualTo(userEmail));
            Assert.That(capturedUserForAdd?.RoleId, Is.EqualTo((int)UserRole.Learner));
            Assert.That(capturedUserForAdd?.FullName, Is.EqualTo(""));
            Assert.That(capturedUserForAdd?.PasswordHash, Is.Null.Or.Empty);

            Assert.That(capturedUserForToken, Is.Not.Null);
            Assert.That(capturedUserForToken?.Email, Is.EqualTo(userEmail));
        });

        _mockOAuthServiceFactory.Verify(f => f.Create(OAuthProvider.Google), Times.Once);
        _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u =>
            u.Email == userEmail && u.RoleId == (int)UserRole.Learner
        )), Times.Once);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(userEmail), Times.Exactly(2));
        _mockJwtService.Verify(service => service.GenerateToken(It.Is<User>(u => u.Email == userEmail)), Times.Once);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var request = new SignInRequest(Email: "test@example.com", Password: "password123");
        var userPasswordHash = PasswordHelper.HashPassword(request.Password);
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = userPasswordHash };
        const string expectedToken = "generated_jwt_token";

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync(user);
        _mockJwtService.Setup(service => service.GenerateToken(user))
            .Returns(expectedToken);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "Result should be successful.");
            Assert.That(result.Value, Is.EqualTo(expectedToken), "Token should match the expected token.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code should be OK.");
        });
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockJwtService.Verify(service => service.GenerateToken(user), Times.Once);
    }

    [Test]
    public async Task LoginAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new SignInRequest(Email: "nonexistent@example.com", Password: "password123");

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "Result should indicate failure.");
            Assert.That(result.Error, Is.EqualTo("Null user"), "Error message should indicate null user.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Status code should be NotFound.");
        });
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockJwtService.Verify(service => service.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task LoginAsync_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new SignInRequest(Email: "test@example.com", Password: "wrongpassword");
        var correctPasswordHash = PasswordHelper.HashPassword("correctpassword"); // Hash the correct password
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = correctPasswordHash };

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "Result should indicate failure.");
            Assert.That(result.Error, Is.EqualTo("Invalid password"),
                "Error message should indicate invalid password.");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Status code should be Unauthorized.");
        });
        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockJwtService.Verify(service => service.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task RegisterAsync_ValidRequest_CallsAddAndSaveChanges()
    {
        // Arrange
        var request = new SignUpRequest(
            Email: "newuser@example.com",
            Password: "password123",
            ConfirmPassword: "password123",
            RoleId: (int)UserRole.Learner
        );

        _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u =>
                u.Email == request.Email &&
                PasswordHelper.VerifyPassword(request.Password, u.PasswordHash!) &&
                u.RoleId == request.RoleId &&
                u.FullName == "" // As per original logic
        )), Times.Once);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void RegisterAsync_PasswordsDoNotMatch_ThrowsArgumentException()
    {
        // Arrange
        var request = new SignUpRequest(
            Email: "newuser@example.com",
            Password: "password123",
            ConfirmPassword: "password456",
            RoleId: (int)UserRole.Learner
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _authService.RegisterAsync(request));
        Assert.That(ex.Message, Is.EqualTo("Password and confirm password do not match."));
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }


    [Test]
    public async Task ResetPasswordAsync_UserExists_UpdatesPasswordAndReturnsSuccess()
    {
        // Arrange
        var request = new ResetPasswordRequest(Email: "test@example.com", NewPassword: "newPassword123");
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = "oldHash" };

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(request.Email))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
        _mockUserRepository.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _authService.ResetPasswordAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(PasswordHelper.VerifyPassword(request.NewPassword, user.PasswordHash!), Is.True,
                "Password hash should be updated to the new password.");
        });

        _mockUserRepository.Verify(repo => repo.GetUserByEmail(request.Email), Times.Once);
        _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ResetPasswordAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new ResetPasswordRequest(Email: "nonexistent@example.com", NewPassword: "newPassword123");

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