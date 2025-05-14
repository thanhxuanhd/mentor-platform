using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace WebAPI.Test;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUserService> _userServiceMock;
    private UsersController _usersController;

    [SetUp]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _usersController = new UsersController(_userServiceMock.Object);
    }

    [Test]
    public async Task GetUserById_UserExists_ReturnsOkResultWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userResponse = new GetUserResponse { Id = userId, FullName = "Test User", Email = "test@example.com", Role = "Learner" };
        var serviceResult = Result.Success(userResponse, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _usersController.GetUserById(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetUserById_UserDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = Result.Failure<GetUserResponse>("User not found", HttpStatusCode.NotFound);

        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _usersController.GetUserById(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task FilterUser_UsersFound_ReturnsOkResultWithPaginatedUsers()
    {
        // Arrange
        var request = new UserFilterPagedRequest { PageIndex = 1, PageSize = 5, FullName = "Test" };
        var userResponses = new List<GetUserResponse>
        {
            new GetUserResponse { Id = Guid.NewGuid(), FullName = "Test User 1", Email = "test1@example.com", Role = "Learner" },
            new GetUserResponse { Id = Guid.NewGuid(), FullName = "Test User 2", Email = "test2@example.com", Role = "Admin" }
        };
        var paginatedList = new PaginatedList<GetUserResponse>(userResponses, userResponses.Count, request.PageIndex, request.PageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.FilterUserAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _usersController.FilterUser(request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.FilterUserAsync(request), Times.Once);
    }

    [Test]
    public async Task FilterUser_NoUsersFound_ReturnsOkResultWithEmptyPaginatedList()
    {
        // Arrange
        var request = new UserFilterPagedRequest { PageIndex = 1, PageSize = 5, FullName = "NonExistent" };
        var emptyUserResponses = new List<GetUserResponse>();
        var paginatedList = new PaginatedList<GetUserResponse>(emptyUserResponses, 0, request.PageIndex, request.PageSize);
        var serviceResult = Result.Success(paginatedList, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.FilterUserAsync(request))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _usersController.FilterUser(request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.FilterUserAsync(request), Times.Once);
    }

    [Test]
    public async Task EditUser_UserExistsAndRequestIsValid_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new EditUserRequest { FullName = "Updated Name", Email = "test@example.com", RoleId = 1 };
        var serviceResult = Result.Success<bool>(true, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.EditUserAsync(userId, request))
            .Returns(Task.FromResult(serviceResult));

        // Act
        var result = await _usersController.EditUser(userId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.EditUserAsync(userId, request), Times.Once);
    }

    [Test]
    public async Task EditUser_UserDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new EditUserRequest { FullName = "Updated Name", Email = "test@example.com", RoleId = 1 };
        var serviceResult = Result.Failure<bool>("User not found", HttpStatusCode.NotFound);

        _userServiceMock.Setup(s => s.EditUserAsync(userId, request))
            .Returns(Task.FromResult(serviceResult));

        // Act
        var result = await _usersController.EditUser(userId, request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.EditUserAsync(userId, request), Times.Once);
    }

    [Test]
    public async Task ChangeUserStatus_UserExists_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = Result.Success<bool>(true, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.ChangeUserStatusAsync(userId))
            .Returns(Task.FromResult(serviceResult));

        // Act
        var result = await _usersController.ChangeUserStatus(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.ChangeUserStatusAsync(userId), Times.Once);
    }

    [Test]
    public async Task ChangeUserStatus_UserDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serviceResult = Result.Failure<bool>("User not found", HttpStatusCode.NotFound);

        _userServiceMock.Setup(s => s.ChangeUserStatusAsync(userId))
            .Returns(Task.FromResult(serviceResult));

        // Act
        var result = await _usersController.ChangeUserStatus(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
        _userServiceMock.Verify(s => s.ChangeUserStatusAsync(userId), Times.Once);
    }
}
