using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.FilterUserAsync(request), Times.Once);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.FilterUserAsync(request), Times.Once);
        });
    }

    [Test]
    public async Task EditUser_UserExistsAndRequestIsValid_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new EditUserRequest { FullName = "Updated Name", Email = "test@example.com", Role = "Admin" };
        var serviceResult = Result.Success<bool>(true, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.EditUserAsync(userId, request))
            .Returns(Task.FromResult(serviceResult));

        // Act
        var result = await _usersController.EditUser(userId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            if (objectResult != null)
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            }
            _userServiceMock.Verify(s => s.EditUserAsync(userId, request), Times.Once);
        });
    }

    [Test]
    public async Task EditUser_UserDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new EditUserRequest { FullName = "Updated Name", Email = "test@example.com", Role = "Admin" };
        var serviceResult = Result.Failure<bool>("User not found", HttpStatusCode.NotFound);

        _userServiceMock.Setup(s => s.EditUserAsync(userId, request))
            .Returns(Task.FromResult(serviceResult));

        // Act
        var result = await _usersController.EditUser(userId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            if (objectResult != null)
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
                Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            }
            _userServiceMock.Verify(s => s.EditUserAsync(userId, request), Times.Once);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            if (objectResult != null)
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            }
            _userServiceMock.Verify(s => s.ChangeUserStatusAsync(userId), Times.Once);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            if (objectResult != null)
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
                Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            }
            _userServiceMock.Verify(s => s.ChangeUserStatusAsync(userId), Times.Once);
        });
    }

    [Test]
    public async Task UploadAvatar_WhenSuccess_ReturnsOkWithUrl()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockFile = new Mock<IFormFile>();
        var expectedUrl = "http://localhost/images/avatar.jpg";

        var httpRequestMock = new Mock<HttpRequest>();
        httpRequestMock.Setup(r => r.Scheme).Returns("http");
        httpRequestMock.Setup(r => r.Host).Returns(new HostString("localhost"));

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controllerContext.HttpContext.Request.Scheme = "http";
        controllerContext.HttpContext.Request.Host = new HostString("localhost");

        _usersController.ControllerContext = controllerContext;

        _userServiceMock
            .Setup(s => s.UploadAvatarAsync(userId, It.IsAny<HttpRequest>(), mockFile.Object))
            .ReturnsAsync(Result.Success(expectedUrl, HttpStatusCode.OK));

        // Act
        var result = await _usersController.UploadAvatar(userId, mockFile.Object) as ObjectResult;
        var resultValue = result?.Value as Result<string>;
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue!.IsSuccess, Is.True);
            Assert.That(resultValue.Value, Is.EqualTo(expectedUrl));
        });
    }

    [Test]
    public async Task UploadAvatar_WhenFail_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockFile = new Mock<IFormFile>();

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controllerContext.HttpContext.Request.Scheme = "http";
        controllerContext.HttpContext.Request.Host = new HostString("localhost");

        _usersController.ControllerContext = controllerContext;

        _userServiceMock
            .Setup(s => s.UploadAvatarAsync(userId, It.IsAny<HttpRequest>(), mockFile.Object))
            .ReturnsAsync(Result.Failure<string>("File not selected", HttpStatusCode.BadRequest));

        // Act
        var result = await _usersController.UploadAvatar(userId, mockFile.Object) as ObjectResult;
        var resultValue = result?.Value as Result<string>;
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue!.IsSuccess, Is.False);
            Assert.That(resultValue.Error, Is.EqualTo("File not selected"));
        });
    }

    [Test]
    public void RemoveAvatar_Success_ReturnsOk()
    {
        // Arrange
        var imageUrl = "http://localhost/images/avatar.jpg";
        var result = Result.Success(true, HttpStatusCode.OK);

        _userServiceMock.Setup(s => s.RemoveAvatar(imageUrl)).Returns(result);

        // Act
        var response = _usersController.RemoveAvatar(imageUrl) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(response.Value, Is.EqualTo(result));
        });
    }

    [Test]
    public void RemoveAvatar_InvalidUrl_ReturnsBadRequest()
    {
        // Arrange
        var imageUrl = "invalid-url";
        var result = Result.Failure<bool>("Invalid image URL format.", HttpStatusCode.BadRequest);

        _userServiceMock.Setup(s => s.RemoveAvatar(imageUrl)).Returns(result);

        // Act
        var response = _usersController.RemoveAvatar(imageUrl) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(response.Value, Is.EqualTo(result));
        });
    }

    [Test]
    public void RemoveAvatar_FileNotFound_ReturnsNotFound()
    {
        // Arrange
        var imageUrl = "http://localhost/images/nonexistent.jpg";
        var result = Result.Failure<bool>("Avatar file not found.", HttpStatusCode.NotFound);

        _userServiceMock.Setup(s => s.RemoveAvatar(imageUrl)).Returns(result);

        // Act
        var response = _usersController.RemoveAvatar(imageUrl) as ObjectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(response.Value, Is.EqualTo(result));
        });
    }

    [Test]
    public async Task UploadMentorDocument_WhenSuccess_ReturnsOkWithUrl()
    {
        // Arrange
        var testUserIdGuid = Guid.NewGuid();
        var testUserIdString = testUserIdGuid.ToString();
        var mockFile = new Mock<IFormFile>();
        var expectedUrl = "http://localhost/documents/mentor_document.pdf";
        var serviceResult = Result.Success(expectedUrl, HttpStatusCode.OK);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUserIdString),
            new Claim(ClaimTypes.Role, "Mentor") // Role is checked by [Authorize(Roles = "Mentor")]
        }, "TestAuthentication"));

        var httpContext = new DefaultHttpContext { User = user };
        // Setup request properties as the service might use them (e.g., to construct URLs)
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");

        _usersController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _userServiceMock
            .Setup(s => s.UploadDocumentAsync(testUserIdGuid, httpContext.Request, mockFile.Object))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.UploadMentorDocument(mockFile.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

            var returnedResult = objectResult.Value as Result<string>;
            Assert.That(returnedResult, Is.Not.Null, "Value of ObjectResult should be Result<string>.");
            Assert.That(returnedResult!.IsSuccess, Is.True);
            Assert.That(returnedResult.Value, Is.EqualTo(expectedUrl));

            _userServiceMock.Verify(s => s.UploadDocumentAsync(testUserIdGuid, httpContext.Request, mockFile.Object), Times.Once);
        });
    }

    [Test]
    public async Task UploadMentorDocument_WhenServiceFails_ReturnsStatusCodeFromService()
    {
        // Arrange
        var testUserIdGuid = Guid.NewGuid();
        var testUserIdString = testUserIdGuid.ToString();
        var mockFile = new Mock<IFormFile>();
        var errorMessage = "Upload failed due to invalid file type.";
        var serviceResult = Result.Failure<string>(errorMessage, HttpStatusCode.BadRequest);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUserIdString),
            new Claim(ClaimTypes.Role, "Mentor")
        }, "TestAuthentication"));

        var httpContext = new DefaultHttpContext { User = user };
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");

        _usersController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _userServiceMock
            .Setup(s => s.UploadDocumentAsync(testUserIdGuid, httpContext.Request, mockFile.Object))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.UploadMentorDocument(mockFile.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));

            var returnedResult = objectResult.Value as Result<string>;
            Assert.That(returnedResult, Is.Not.Null, "Value of ObjectResult should be Result<string>.");
            Assert.That(returnedResult!.IsSuccess, Is.False);
            Assert.That(returnedResult.Error, Is.EqualTo(errorMessage));

            _userServiceMock.Verify(s => s.UploadDocumentAsync(testUserIdGuid, httpContext.Request, mockFile.Object), Times.Once);
        });
    }

    [Test]
    public async Task RemoveMentorDocument_WhenSuccess_ReturnsOk()
    {
        // Arrange
        var testUserIdGuid = Guid.NewGuid();
        var testUserIdString = testUserIdGuid.ToString();
        var documentUrl = "http://localhost/documents/mentor_document.pdf";
        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUserIdString),
            new Claim(ClaimTypes.Role, "Mentor")
        }, "TestAuthentication"));

        _usersController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _userServiceMock
            .Setup(s => s.RemoveDocumentAsync(testUserIdGuid, documentUrl))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.RemoveMentorDocument(documentUrl);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var returnedResult = objectResult.Value as Result<bool>;
            Assert.That(returnedResult, Is.Not.Null, "Value of ObjectResult should be Result<bool>.");
            Assert.That(returnedResult!.IsSuccess, Is.True);
            Assert.That(returnedResult.Value, Is.True);
            _userServiceMock.Verify(s => s.RemoveDocumentAsync(testUserIdGuid, documentUrl), Times.Once);
        });
    }

    [Test]
    public async Task RemoveMentorDocument_WhenServiceFails_ReturnsStatusCodeFromService()
    {
        // Arrange
        var testUserIdGuid = Guid.NewGuid();
        var testUserIdString = testUserIdGuid.ToString();
        var documentUrl = "http://localhost/documents/nonexistent_document.pdf";
        var errorMessage = "Document not found";
        var serviceResult = Result.Failure<bool>(errorMessage, HttpStatusCode.NotFound);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUserIdString),
            new Claim(ClaimTypes.Role, "Mentor")
        }, "TestAuthentication"));

        _usersController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _userServiceMock
            .Setup(s => s.RemoveDocumentAsync(testUserIdGuid, documentUrl))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.RemoveMentorDocument(documentUrl);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            var returnedResult = objectResult.Value as Result<bool>;
            Assert.That(returnedResult, Is.Not.Null, "Value of ObjectResult should be Result<bool>.");
            Assert.That(returnedResult!.IsSuccess, Is.False);
            Assert.That(returnedResult.Error, Is.EqualTo(errorMessage));
            _userServiceMock.Verify(s => s.RemoveDocumentAsync(testUserIdGuid, documentUrl), Times.Once);
        });
    }

    [Test]
    public async Task EditUserDetailAsync_WhenSuccess_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new EditUserProfileRequest(
            FullName: "Test User",
            RoleId: 1, // Assuming 1 is a valid RoleId, e.g., Admin or Learner
            Bio: "This is a test bio for the user.",
            ProfilePhotoUrl: "http://example.com/profile.jpg",
            PhoneNumber: "1234567890",
            Skills: "C#, ASP.NET Core",
            Experiences: "5 years in software development",
            PreferredCommunicationMethod: CommunicationMethod.AudioCall,
            Goal: "To become a senior developer.",
            PreferredSessionFrequency: SessionFrequency.EveryTwoWeeks,
            PreferredSessionDuration: 30, // e.g., 30 minutes
            PreferredLearningStyle: LearningStyle.Visual,
            IsPrivate: false,
            IsAllowedMessage: true,
            IsReceiveNotification: true,
            AvailabilityIds: new List<Guid>(),
            ExpertiseIds: new List<Guid>(),
            TeachingApproachIds: new List<Guid>(),
            CategoryIds: new List<Guid>()
        );

        var serviceResult = Result.Success(true, HttpStatusCode.OK);

        _userServiceMock
            .Setup(s => s.EditUserDetailAsync(userId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.EditUserDetailAsync(userId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.EditUserDetailAsync(userId, request), Times.Once);
        });
    }

    [Test]
    public async Task EditUserDetailAsync_WhenServiceFails_ReturnsStatusCodeFromService()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new EditUserProfileRequest(
            FullName: "Test User Update Attempt",
            RoleId: 3, // Another valid RoleId
            Bio: "Attempting to update bio for a user that might not exist.",
            ProfilePhotoUrl: "http://example.com/attempt_profile.png",
            PhoneNumber: "0987654321",
            Skills: "Java, Spring",
            Experiences: "2 years as Junior Developer",
            PreferredCommunicationMethod: CommunicationMethod.VideoCall,
            Goal: "To understand microservices.",
            PreferredSessionFrequency: SessionFrequency.Weekly,
            PreferredSessionDuration: 45, // e.g., 45 minutes
            PreferredLearningStyle: LearningStyle.Auditory,
            IsPrivate: true,
            IsAllowedMessage: false,
            IsReceiveNotification: false,
            AvailabilityIds: new List<Guid>(),
            ExpertiseIds: new List<Guid>(),
            TeachingApproachIds: new List<Guid>(),
            CategoryIds: new List<Guid>()
        );
        var errorMessage = "User not found to update details.";
        var serviceResult = Result.Failure<bool>(errorMessage, HttpStatusCode.NotFound);

        _userServiceMock
            .Setup(s => s.EditUserDetailAsync(userId, request))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.EditUserDetailAsync(userId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.EditUserDetailAsync(userId, request), Times.Once);
        });
    }

    [Test]
    public async Task GetUserDetailAsync_WhenUserExists_ReturnsOkWithUserDetails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDetailResponse = new GetUserDetailResponse(
            FullName: "Test User",
            RoleId: 1,
            Bio: "This is a test bio for the user.",
            ProfilePhotoUrl: "http://example.com/profile.jpg",
            PhoneNumber: "1234567890",
            JoinedDate: DateOnly.FromDateTime(DateTime.Now), 
            Skills: "C#, ASP.NET Core",
            Experiences: "5 years in software development",
            PreferredCommunicationMethod: CommunicationMethod.AudioCall,
            Goal: "To become a senior developer.",
            PreferredSessionFrequency: SessionFrequency.EveryTwoWeeks,
            PreferredSessionDuration: 30, // e.g., 30 minutes
            PreferredLearningStyle: LearningStyle.Visual,
            IsPrivate: false,
            IsAllowedMessage: true,
            IsReceiveNotification: true,
            AvailabilityIds: new List<Guid>(),
            ExpertiseIds: new List<Guid>(),
            TeachingApproachIds: new List<Guid>(),
            CategoryIds: new List<Guid>()
        );
        
        var serviceResult = Result.Success(userDetailResponse, HttpStatusCode.OK);

        _userServiceMock
            .Setup(s => s.GetUserDetailAsync(userId))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.GetUserDetailAsync(userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.GetUserDetailAsync(userId), Times.Once);
        });
    }

    [Test]
    public async Task GetUserDetailAsync_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var errorMessage = "User details not found.";
        var serviceResult = Result.Failure<GetUserDetailResponse>(errorMessage, HttpStatusCode.NotFound);

        _userServiceMock
            .Setup(s => s.GetUserDetailAsync(userId))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.GetUserDetailAsync(userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null, "Result should be ObjectResult.");
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.GetUserDetailAsync(userId), Times.Once);
        });
    }

    [Test]
    public async Task GetUserByEmail_WhenUserExists_ReturnsOkWithUser()
    {
        // Arrange
        var email = "test@example.com";
        var userResponse = new GetUserResponse { Email = email, FullName = "Test User" };
        var serviceResult = Result.Success(userResponse, HttpStatusCode.OK);

        _userServiceMock
            .Setup(s => s.GetUserByEmailAsync(email))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.GetUserByEmail(email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.GetUserByEmailAsync(email), Times.Once);
        });
    }

    [Test]
    public async Task ForgotPasswordRequest_WhenEmailExists_ReturnsOk()
    {
        // Arrange
        var email = "user@example.com";
        var serviceResult = Result.Success(true, HttpStatusCode.OK); // Assuming success means true

        _userServiceMock
            .Setup(s => s.ForgotPasswordRequest(email))
            .ReturnsAsync(serviceResult);

        // Act
        var actionResult = await _usersController.ForgotPasswordRequest(email);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _userServiceMock.Verify(s => s.ForgotPasswordRequest(email), Times.Once);
        });
    }
}
