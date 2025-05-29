using Application.Services.MentorApplications;
using Contract.Dtos.Users.Requests;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Net;
using System.Security.Claims;

namespace MentorPlatformAPI.Tests.Controllers
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
    }
}
