using Application.Services.MentorApplications;
using Contract.Dtos.Users.Requests;
using Contract.Repositories;
using Contract.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Application.Test
{
    public class MentorApplicationServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMentorApplicationRepository> _mentorApplicationRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<ILogger<MentorApplicationService>> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private MentorApplicationService _mentorApplicationService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mentorApplicationRepositoryMock = new Mock<IMentorApplicationRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            // Setup a default WebRootPath for testing
            _webHostEnvironmentMock.Setup(x => x.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            _loggerMock = new Mock<ILogger<MentorApplicationService>>();
            _httpRequestMock = new Mock<HttpRequest>();

            _mentorApplicationService = new MentorApplicationService(
                _userRepositoryMock.Object,
                _mentorApplicationRepositoryMock.Object,
                _emailServiceMock.Object,
                _webHostEnvironmentMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task CreateMentorApplicationAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            MentorSubmissionRequest request = new MentorSubmissionRequest(null, null, null, null, null);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, null)).ReturnsAsync((User)null);

            // Act
            var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, request, _httpRequestMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo("User not found"));
            });
        }

        [Test]
        public async Task CreateMentorApplicationAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            MentorSubmissionRequest request = new MentorSubmissionRequest("education", "experience", "certifications", "statement", null);
            var user = new User { Id = userId, Experiences = "old experience" };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, null)).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.Update(It.IsAny<User>()));
            _mentorApplicationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<MentorApplication>()));
            _mentorApplicationRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, request, _httpRequestMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(user.Experiences, Is.EqualTo(request.WorkExperience));
            });
        }

        [Test]
        public async Task CreateMentorApplicationAsync_WithDocuments_ReturnsSuccess()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            var fileList = new List<IFormFile> { fileMock.Object };
            MentorSubmissionRequest request = new MentorSubmissionRequest("education", "experience", "certifications", "statement", fileList);
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, null)).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.Update(It.IsAny<User>()));
            _mentorApplicationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<MentorApplication>()));
            _mentorApplicationRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            // _webHostEnvironmentMock.Setup(x => x.WebRootPath).Returns("wwwroot"); // Moved to Setup
            _httpRequestMock.Setup(x => x.Scheme).Returns("http");
            _httpRequestMock.Setup(x => x.Host).Returns(new HostString("localhost"));

            // Act
            var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, request, _httpRequestMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });
        }

        [Test]
        public async Task CreateMentorApplicationAsync_InvalidContentType_ReturnsBadRequest()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("image/mp4"); // Invalid content type
            fileMock.Setup(f => f.Length).Returns(100);

            var fileList = new List<IFormFile> { fileMock.Object };
            MentorSubmissionRequest request = new MentorSubmissionRequest("education", "experience", "certifications", "statement", fileList);
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, null)).ReturnsAsync(user);

            // Act
            var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, request, _httpRequestMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File content type is not allowed."));
            });
        }

        [Test]
        public async Task CreateMentorApplicationAsync_FileSizeExceedsLimit_ReturnsBadRequest()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.Length).Returns(2 * 1024 * 1024); // 2MB, exceeding the limit

            var fileList = new List<IFormFile> { fileMock.Object };
            MentorSubmissionRequest request = new MentorSubmissionRequest("education", "experience", "certifications", "statement", fileList);
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, null)).ReturnsAsync(user);

            // Act
            var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, request, _httpRequestMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File size must not exceed 1MB."));
            });
        }
    }
}