using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace WebAPI.Test;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock;
    private AuthController _authController;

    [SetUp]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Test]
    public async Task SignInUser_ValidRequest_ReturnsOkResultWithToken()
    {
        // Arrange
        var request = new SignInRequest(Email: "test@example.com", Password: "password");
        var serviceResult = Result.Success("token", HttpStatusCode.OK);

        _authServiceMock.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.SignInUser(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task SignInUser_InvalidCredentials_ReturnsUnauthorizedResult()
    {
        // Arrange
        var request = new SignInRequest(Email: "invalid@example.com", Password: "wrongpassword");
        var serviceResult = Result.Failure<string>("Invalid credentials", HttpStatusCode.Unauthorized);

        _authServiceMock.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.SignInUser(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task SignUpUser_ValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new SignUpRequest(Email: "newuser@example.com", Password: "password", ConfirmPassword: "New User", RoleId: 1);

        _authServiceMock.Setup(s => s.RegisterAsync(request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authController.SignUpUser(request);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        _authServiceMock.Verify(s => s.RegisterAsync(request), Times.Once);
    }

    [Test]
    public async Task SignInGithub_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new OAuthSignInRequest(Token: "github_token");
        var serviceResult = Result.Success("github_user_data", HttpStatusCode.OK);

        _authServiceMock.Setup(s => s.LoginGithubAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.SignInGithub(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(serviceResult));
        _authServiceMock.Verify(s => s.LoginGithubAsync(request), Times.Once);
    }

    [Test]
    public async Task SignInGoogle_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new OAuthSignInRequest(Token: "google_token");
        var serviceResult = Result.Success("google_user_data", HttpStatusCode.OK);

        _authServiceMock.Setup(s => s.LoginGoogleAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.SignInGoogle(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(serviceResult));
        _authServiceMock.Verify(s => s.LoginGoogleAsync(request), Times.Once);
    }

    [Test]
    public async Task ResetPassword_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new ResetPasswordRequest(Email: "test@example.com", OldPassword: "oldpassword", NewPassword: "newpassword");
        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        _authServiceMock.Setup(s => s.ResetPasswordAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.ResetPassword(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _authServiceMock.Verify(s => s.ResetPasswordAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task ResetPassword_InvalidRequest_ReturnsBadRequestResult()
    {
        // Arrange
        var request = new ResetPasswordRequest(Email: "nonexistent@example.com", OldPassword: "oldpassword", NewPassword: "newpassword");
        var serviceResult = Result.Failure<bool>("User not found", HttpStatusCode.BadRequest);

        _authServiceMock.Setup(s => s.ResetPasswordAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.ResetPassword(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _authServiceMock.Verify(s => s.ResetPasswordAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task CheckEmailExists_EmailExists_ReturnsOkResult()
    {
        // Arrange
        var email = "test@example.com";
        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        _authServiceMock.Setup(s => s.CheckEmailExistsAsync(email))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.CheckEmailExists(email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _authServiceMock.Verify(s => s.CheckEmailExistsAsync(email), Times.Once);
        });
    }

    [Test]
    public async Task CheckEmailExists_EmailDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var serviceResult = Result.Failure<bool>("Email not found", HttpStatusCode.NotFound);

        _authServiceMock.Setup(s => s.CheckEmailExistsAsync(email))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _authController.CheckEmailExists(email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _authServiceMock.Verify(s => s.CheckEmailExistsAsync(email), Times.Once);
        });
    }
}