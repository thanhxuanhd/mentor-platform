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
            .Callback<User>(u => {
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
            Assert.That(capturedUserForAdd?.PasswordHash, Is.Null.Or.Empty); // OAuth users might not have a password hash initially

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
            .Callback<User>(u => {
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
}