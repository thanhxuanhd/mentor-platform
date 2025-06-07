using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Shared;
using Domain.Enums;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Net;
using System.Security.Claims;

namespace WebAPI.Test
{
    public class MentorApplicationControllerTests
    {
        private Mock<IMentorApplicationService> _mentorApplicationServiceMock;
        private MentorApplicationController _controller;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private DefaultHttpContext _httpContext;
        private Mock<HttpRequest> _httpRequestMock;

        [SetUp]
        public void Setup()
        {
            _mentorApplicationServiceMock = new Mock<IMentorApplicationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContext = new DefaultHttpContext();
            _httpRequestMock = new Mock<HttpRequest>();

            // Setup user claims  
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _httpContext.User = principal;
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);

            // Use a workaround to mock the HttpRequest  
            _httpContextAccessorMock.Setup(x => x.HttpContext.Request).Returns(_httpRequestMock.Object);

            _controller = new MentorApplicationController(_mentorApplicationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext,
                    ActionDescriptor = new ControllerActionDescriptor(),
                    RouteData = new RouteData()
                }
            };
        }

        [Test]
        public async Task MentorSubmission_ValidRequest_ReturnsStatusCodeFromService()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new MentorSubmissionRequest("education", "experience", "certifications", "statement", null);
            var expectedStatusCode = HttpStatusCode.OK;
            var resultFromService = Result.Success(true, expectedStatusCode);

            _mentorApplicationServiceMock.Setup(x => x.CreateMentorApplicationAsync(It.IsAny<Guid>(), It.IsAny<MentorSubmissionRequest>(), It.IsAny<HttpRequest>()))
                .ReturnsAsync(resultFromService);

            // Act
            var actionResult = await _controller.MentorSubmission(request) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
                Assert.That(actionResult.Value, Is.EqualTo(resultFromService));
            });
        }

        [Test]
        public async Task MentorSubmission_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new MentorSubmissionRequest(null, null, null, null, null);
            var expectedStatusCode = HttpStatusCode.BadRequest;
            var errorMessage = "Invalid request";
            var resultFromService = Result.Failure<bool>(errorMessage, expectedStatusCode);

            _mentorApplicationServiceMock.Setup(x => x.CreateMentorApplicationAsync(It.IsAny<Guid>(), It.IsAny<MentorSubmissionRequest>(), It.IsAny<HttpRequest>()))
                .ReturnsAsync(resultFromService);

            // Act
            var actionResult = await _controller.MentorSubmission(request) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
                Assert.That(actionResult.Value, Is.EqualTo(resultFromService));
            });
        }


        [Test]
        public async Task GetAllMentorApplications_ReturnsOkResult_WithPaginatedApplications()
        {
            // Arrange
            var request = new FilterMentorApplicationRequest();
            var response = new PaginatedList<FilterMentorApplicationResponse>(new List<FilterMentorApplicationResponse>(), 0, 1, 10);
            var result = Result.Success(response, HttpStatusCode.OK);
            _mentorApplicationServiceMock.Setup(s => s.GetAllMentorApplicationsAsync(request)).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetAllMentorApplications(request);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)actionResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(result));
        }

        [Test]
        public async Task GetMentorApplicationById_ReturnsOkResult_WithApplicationDetail()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var response = new MentorApplicationDetailResponse { MentorApplicationId = applicationId };
            var result = Result.Success(response, HttpStatusCode.OK);
            _mentorApplicationServiceMock.Setup(s => s.GetMentorApplicationByIdAsync(It.IsAny<Guid>(), applicationId)).ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetMentorApplicationById(applicationId);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = (ObjectResult)actionResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(result));
        }
        [Test]
        public async Task GetMentorApplicationByMentorId_ReturnsOkResult_WithListOfApplications()
        {
            // Arrange
            var mentorId = Guid.NewGuid();
            var responseList = new List<FilterMentorApplicationResponse>
            {
                new FilterMentorApplicationResponse { MentorApplicationId = Guid.NewGuid(), MentorName = "Mentor 1" }
            };
            var expectedResult = Result.Success(responseList, HttpStatusCode.OK);

            _mentorApplicationServiceMock
                .Setup(s => s.GetListMentorApplicationByMentorIdAsync(mentorId))
                .ReturnsAsync(expectedResult);

            // Act
            var actionResult = await _controller.GetMentorApplicationByMentorId(mentorId) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
            });
            _mentorApplicationServiceMock.Verify(s => s.GetListMentorApplicationByMentorIdAsync(mentorId), Times.Once);
        }

        [Test]
        public async Task RequestApplicationInfo_ValidRequest_ReturnsStatusCodeFromService()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var adminIdClaimValue = _controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Assert.That(adminIdClaimValue, Is.Not.Null, "NameIdentifier claim must be set in Setup.");
            var adminId = Guid.Parse(adminIdClaimValue!);

            var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };
            var serviceResponse = new RequestApplicationInfoResponse { Message = "Info requested" };
            var expectedResult = Result.Success(serviceResponse, HttpStatusCode.OK);

            _mentorApplicationServiceMock
                .Setup(s => s.RequestApplicationInfoAsync(adminId, applicationId, request))
                .ReturnsAsync(expectedResult);

            // Act
            var actionResult = await _controller.RequestApplicationInfo(applicationId, request) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
            });
            _mentorApplicationServiceMock.Verify(s => s.RequestApplicationInfoAsync(adminId, applicationId, request), Times.Once);
        }

        [Test]
        public async Task UpdateApplicationStatus_ValidRequest_ReturnsStatusCodeFromService()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var adminIdClaimValue = _controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Assert.That(adminIdClaimValue, Is.Not.Null, "NameIdentifier claim must be set in Setup.");
            var adminId = Guid.Parse(adminIdClaimValue!);

            var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Approved" };
            var serviceResponse = new UpdateApplicationStatusResponse { Message = "Status updated" };
            var expectedResult = Result.Success(serviceResponse, HttpStatusCode.OK);

            _mentorApplicationServiceMock
                .Setup(s => s.UpdateApplicationStatusAsync(adminId, applicationId, request))
                .ReturnsAsync(expectedResult);

            // Act
            var actionResult = await _controller.UpdateApplicationStatus(applicationId, request) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(actionResult.Value, Is.EqualTo(expectedResult));
            });
            _mentorApplicationServiceMock.Verify(s => s.UpdateApplicationStatusAsync(adminId, applicationId, request), Times.Once);
        }

        [Test]
        public async Task EditMentorApplication_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var request = new UpdateMentorApplicationRequest
            {
                WorkExperience = "Updated Work Experience",
                Education = "Updated Education",
                Certifications = "Updated Certifications",
                Statement = "Updated Statement",
                Documents = new FormFileCollection()
            };

            var expectedStatusCode = HttpStatusCode.OK;
            var serviceResult = Result.Success(true, expectedStatusCode);

            _mentorApplicationServiceMock
                .Setup(s => s.EditMentorApplicationAsync(applicationId, request, It.IsAny<HttpRequest>()))
                .ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.EditMentorApplication(applicationId, request) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
                Assert.That(actionResult.Value, Is.EqualTo(serviceResult));
            });
            _mentorApplicationServiceMock.Verify(s => s.EditMentorApplicationAsync(applicationId, request, _httpContext.Request), Times.Once);
        }

        [Test]
        public async Task EditMentorApplication_ApplicationNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var request = new UpdateMentorApplicationRequest
            {
                WorkExperience = "Updated Work Experience",
                Education = "Updated Education",
                Certifications = "Updated Certifications",
                Statement = "Updated Statement",
                Documents = new FormFileCollection()
            };

            var expectedStatusCode = HttpStatusCode.NotFound;
            var errorMessage = "Mentor application not found.";
            var serviceResult = Result.Failure<bool>(errorMessage, expectedStatusCode);

            _mentorApplicationServiceMock
                .Setup(s => s.EditMentorApplicationAsync(applicationId, request, It.IsAny<HttpRequest>()))
                .ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.EditMentorApplication(applicationId, request) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.Not.Null);
                Assert.That(actionResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
                Assert.That(actionResult.Value, Is.EqualTo(serviceResult));
            });
            _mentorApplicationServiceMock.Verify(s => s.EditMentorApplicationAsync(applicationId, request, _httpContext.Request), Times.Once);
        }

        [Test]
        public async Task EditMentorApplication_ServiceReturnsConflict_ReturnsConflictResult()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var request = new UpdateMentorApplicationRequest
            {
                WorkExperience = "Updated Work Experience",
                Education = "Updated Education",
                Certifications = "Updated Certifications",
                Statement = "Updated Statement",
                Documents = new FormFileCollection()
            };
            var expectedStatusCode = HttpStatusCode.Conflict;
            var serviceResult = Result.Failure<bool>("Cannot update application in this state.", expectedStatusCode);
            _mentorApplicationServiceMock.Setup(s => s.EditMentorApplicationAsync(applicationId, request, It.IsAny<HttpRequest>())).ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.EditMentorApplication(applicationId, request) as ObjectResult;

            // Assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo((int)expectedStatusCode));
            Assert.That(actionResult.Value, Is.EqualTo(serviceResult));
            _mentorApplicationServiceMock.Verify(s => s.EditMentorApplicationAsync(applicationId, request, _httpContext.Request), Times.Once);
        }
    }
}