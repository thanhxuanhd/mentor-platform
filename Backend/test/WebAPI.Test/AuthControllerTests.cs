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