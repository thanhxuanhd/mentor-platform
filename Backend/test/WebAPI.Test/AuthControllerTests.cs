using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Contract.Dtos.Authentication.Responses;
using Contract.Shared;
using Domain.Enums;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace WebAPI.Test;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService;
    private AuthController _authController;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _authController = new AuthController(_mockAuthService.Object);
    }

    [Test]
    public async Task SignInUser_Success_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new SignInRequest("test@example.com", "password");
        var authResponse = new AuthResponse("jwt-token", Guid.NewGuid(), "Active");
        var result = Result.Success(authResponse, HttpStatusCode.OK);
        _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.SignInUser(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(result));
            var resultValue = (Result<AuthResponse>)objectResult.Value!;
            Assert.That(resultValue.Value, Is.EqualTo(authResponse));
        });

        _mockAuthService.Verify(s => s.LoginAsync(request), Times.Once());
    }

    [Test]
    public async Task SignInUser_Failure_ReturnsNotFound()
    {
        // Arrange
        var request = new SignInRequest("test@example.com", "wrongpassword");
        var result = Result.Failure<AuthResponse>("Invalid user", HttpStatusCode.NotFound);
        _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.SignInUser(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult.Value, Is.EqualTo(result));
            var resultValue = (Result<AuthResponse>)objectResult.Value!;
            Assert.That(resultValue.Error, Is.EqualTo("Invalid user"));
        });

        _mockAuthService.Verify(s => s.LoginAsync(request), Times.Once());
    }

    [Test]
    public async Task SignUpUser_Success_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new SignUpRequest("Password123!", "Password123!", "newuser@example.com", (int)UserRole.Learner);
        var authResponse = new AuthResponse("jwt-token", Guid.NewGuid(), "Active");
        var result = Result.Success(authResponse, HttpStatusCode.OK);
        _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.SignUpUser(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(result));
            var resultValue = (Result<AuthResponse>)objectResult.Value!;
            Assert.That(resultValue.Value, Is.EqualTo(authResponse));
        });

        _mockAuthService.Verify(s => s.RegisterAsync(request), Times.Once());
    }

    [Test]
    public async Task SignUpUser_EmailExists_ReturnsBadRequest()
    {
        // Arrange
        var request = new SignUpRequest("Password123!", "Password123!", "existing@example.com", (int)UserRole.Learner);
        var result = Result.Failure<AuthResponse>("User email already existed", HttpStatusCode.BadRequest);
        _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.SignUpUser(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(objectResult.Value, Is.EqualTo(result));
            var resultValue = (Result<AuthResponse>)objectResult.Value!;
            Assert.That(resultValue.Error, Is.EqualTo("User email already existed"));
        });

        _mockAuthService.Verify(s => s.RegisterAsync(request), Times.Once());
    }

    [Test]
    public async Task SignInGithub_Success_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new OAuthSignInRequest("github-token");
        var authResponse = new AuthResponse("jwt-token", Guid.NewGuid(), "Active");
        var result = Result.Success(authResponse, HttpStatusCode.OK);
        _mockAuthService.Setup(s => s.LoginGithubAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.SignInGithub(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(okResult.Value, Is.EqualTo(result));
            var resultValue = (Result<AuthResponse>)okResult.Value!;
            Assert.That(resultValue.Value, Is.EqualTo(authResponse));
        });

        _mockAuthService.Verify(s => s.LoginGithubAsync(request), Times.Once());
    }

    [Test]
    public async Task SignInGoogle_Success_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new OAuthSignInRequest("google-token");
        var authResponse = new AuthResponse("jwt-token", Guid.NewGuid(), "Active");
        var result = Result.Success(authResponse, HttpStatusCode.OK);
        _mockAuthService.Setup(s => s.LoginGoogleAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.SignInGoogle(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(okResult.Value, Is.EqualTo(result));
            var resultValue = (Result<AuthResponse>)okResult.Value!;
            Assert.That(resultValue.Value, Is.EqualTo(authResponse));
        });

        _mockAuthService.Verify(s => s.LoginGoogleAsync(request), Times.Once());
    }

    [Test]
    public async Task ResetPassword_Success_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@example.com", "oldPassword123", "newPassword123");
        var result = Result.Success("Password reset successful", HttpStatusCode.OK);
        _mockAuthService.Setup(s => s.ResetPasswordAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.ResetPassword(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(result));
        });

        _mockAuthService.Verify(s => s.ResetPasswordAsync(request), Times.Once());
    }

    [Test]
    public async Task ResetPassword_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@example.com", "oldPassword123", "newPassword123");
        var result = Result.Failure("User not found", HttpStatusCode.NotFound);
        _mockAuthService.Setup(s => s.ResetPasswordAsync(request)).ReturnsAsync(result);

        // Act
        var actionResult = await _authController.ResetPassword(request);

        // Assert
        Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)actionResult;
        Assert.Multiple(() =>
        {
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult.Value, Is.EqualTo(result));
            var resultValue = (Result)objectResult.Value!;
            Assert.That(resultValue!.Error, Is.EqualTo("User not found"));
        });

        _mockAuthService.Verify(s => s.ResetPasswordAsync(request), Times.Once());
    }

    [Test]
    public async Task CheckEmailExists_EmailExists_ReturnsOkResult()
    {
        // Arrange
        const string email = "test@example.com";
        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        _mockAuthService.Setup(s => s.CheckEmailExistsAsync(email))
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
            _mockAuthService.Verify(s => s.CheckEmailExistsAsync(email), Times.Once);
        });
    }

    [Test]
    public async Task CheckEmailExists_EmailDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        const string email = "nonexistent@example.com";
        var serviceResult = Result.Failure<bool>("Email not found", HttpStatusCode.NotFound);

        _mockAuthService.Setup(s => s.CheckEmailExistsAsync(email))
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
            _mockAuthService.Verify(s => s.CheckEmailExistsAsync(email), Times.Once);
        });
    }
}