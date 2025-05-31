using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Application.Test;

public class MentorApplicationServiceTests
{
    private Mock<IEmailService> _emailServiceMock;
    private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
    private Mock<ILogger<MentorApplicationService>> _loggerMock;
    private Mock<HttpRequest> _httpRequestMock;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IMentorApplicationRepository> _mockMentorApplicationRepository;
    private Mock<IEmailService> _mockEmailService;
    private MentorApplicationService _mentorApplicationService;

    [SetUp]
    public void Setup()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        // Setup a default WebRootPath for testing
        _webHostEnvironmentMock.Setup(x => x.WebRootPath)
            .Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
        _loggerMock = new Mock<ILogger<MentorApplicationService>>();
        _httpRequestMock = new Mock<HttpRequest>();

        _mockUserRepository = new Mock<IUserRepository>();
        _mockMentorApplicationRepository = new Mock<IMentorApplicationRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mentorApplicationService = new MentorApplicationService(
            _mockUserRepository.Object,
            _mockMentorApplicationRepository.Object,
            _emailServiceMock.Object,
            _webHostEnvironmentMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_ReturnsPaginatedListOfApplications()
    {
        // Arrange
        var request = new FilterMentorApplicationRequest { PageIndex = 1, PageSize = 10 };
        var applications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor One" }, Status = ApplicationStatus.Submitted
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor Two" }, Status = ApplicationStatus.Approved
            }
        }.AsQueryable();

        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            applications.Select(x => new FilterMentorApplicationResponse { MentorName = x.Mentor.FullName }).ToList(),
            applications.Count(),
            request.PageIndex,
            request.PageSize
        );

        _mockMentorApplicationRepository.Setup(r => r.GetAllApplicationsAsync()).Returns(applications);
        _mockMentorApplicationRepository.Setup(r =>
                r.ToPaginatedListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>(), request.PageSize,
                    request.PageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null)
            {
                Assert.That(result.Value.Items.Count(), Is.EqualTo(2));
            }

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_WithKeywordFilter_ReturnsFilteredApplications()
    {
        // Arrange
        var request = new FilterMentorApplicationRequest { PageIndex = 1, PageSize = 10, Keyword = "One" };
        var applications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor One" }, Status = ApplicationStatus.Submitted
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor Two" }, Status = ApplicationStatus.Approved
            }
        }.AsQueryable();

        var filteredApplications = applications.Where(x =>
            x.Mentor.FullName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase));
        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            filteredApplications.Select(x => new FilterMentorApplicationResponse { MentorName = x.Mentor.FullName })
                .ToList(),
            filteredApplications.Count(),
            request.PageIndex,
            request.PageSize
        );

        _mockMentorApplicationRepository.Setup(r => r.GetAllApplicationsAsync()).Returns(applications);
        _mockMentorApplicationRepository.Setup(r =>
                r.ToPaginatedListAsync(It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == 1),
                    request.PageSize, request.PageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null)
            {
                Assert.That(result.Value.Items.Count(), Is.EqualTo(1));
                Assert.That(result.Value.Items.First().MentorName, Is.EqualTo("Mentor One"));
            }

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_WithStatusFilter_ReturnsFilteredApplications()
    {
        // Arrange
        var request = new FilterMentorApplicationRequest
        { PageIndex = 1, PageSize = 10, Status = ApplicationStatus.Approved };
        var applications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor One" }, Status = ApplicationStatus.Submitted
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor Two" }, Status = ApplicationStatus.Approved
            }
        }.AsQueryable();

        var filteredApplications = applications.Where(x => x.Status == request.Status.Value);
        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            filteredApplications.Select(x => new FilterMentorApplicationResponse { MentorName = x.Mentor.FullName })
                .ToList(),
            filteredApplications.Count(),
            request.PageIndex,
            request.PageSize
        );

        _mockMentorApplicationRepository.Setup(r => r.GetAllApplicationsAsync()).Returns(applications);
        _mockMentorApplicationRepository.Setup(r =>
                r.ToPaginatedListAsync(It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == 1),
                    request.PageSize, request.PageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null)
            {
                Assert.That(result.Value.Items.Count(), Is.EqualTo(1));
                Assert.That(result.Value.Items.First().MentorName, Is.EqualTo("Mentor Two"));
            }

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_UserIsLearner_ReturnsForbidden()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Learner } };

        _mockUserRepository.Setup(r => r.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            // Assuming Result.Message stores the error string
            Assert.That(result.Error, Is.EqualTo("You do not have permission to view this mentor application."));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Admin } };

        _mockUserRepository.Setup(r => r.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync((MentorApplication?)null);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Mentor application not found."));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_MentorIsNotOwner_ReturnsForbidden()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var otherMentorId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Mentor } };
        var application = new MentorApplication { Id = applicationId, MentorId = otherMentorId, Mentor = new User { Id = otherMentorId } };

        _mockUserRepository.Setup(r => r.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("You do not have permission to view this mentor application."));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_ValidRequest_ReturnsApplicationDetails()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Admin } };
        var expertiseId = Guid.NewGuid();
        var mentorUserId = Guid.NewGuid();
        var mentorUser = new User
        {
            Id = mentorUserId,
            FullName = "Mentor Name",
            Email = "mentor@example.com",
            Bio = "Mentor Bio",
            Experiences = "Some experiences",
            ProfilePhotoUrl = "url/photo.jpg",
            UserExpertises = new List<UserExpertise> { new UserExpertise { UserId = mentorUserId, ExpertiseId = expertiseId, Expertise = new Expertise { Id = expertiseId, Name = "C#" } } }
        };
        var application = new MentorApplication
        {
            Id = applicationId,
            MentorId = mentorUser.Id,
            Mentor = mentorUser,
            Status = ApplicationStatus.Submitted,
            SubmittedAt = DateTime.UtcNow.AddDays(-1),
            ApplicationDocuments = new List<ApplicationDocument>
            {
                new ApplicationDocument { Id = Guid.NewGuid(), DocumentType = FileType.Pdf, DocumentUrl = "url/cv.pdf" } // Using DocumentType from Domain.Enums via using statement
            }
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null)
            {
                Assert.That(result.Value.MentorApplicationId, Is.EqualTo(applicationId));
                Assert.That(result.Value.MentorName, Is.EqualTo(mentorUser.FullName));
                Assert.That(result.Value.Email, Is.EqualTo(mentorUser.Email));
                Assert.That(result.Value.Expertises.Count(), Is.EqualTo(1));
                Assert.That(result.Value.Expertises.First(), Is.EqualTo("C#"));
                Assert.That(result.Value.Documents.Count(), Is.EqualTo(1));
            }
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync((MentorApplication?)null);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            // Assert.That(result.Message, Is.EqualTo("Mentor application not found.")); // Adjust if Message property is different
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_ApplicationNotSubmitted_ReturnsConflict()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };
        var application = new MentorApplication { Id = applicationId, Status = ApplicationStatus.Approved, Mentor = new User() }; // Not Submitted

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            // Assert.That(result.Message, Is.EqualTo("You can only request additional information for submitted applications.")); // Adjust
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_EmailSendFails_ReturnsInternalServerError()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };
        var mentorEmail = "mentor@example.com";
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted,
            Mentor = new User { Email = mentorEmail, FullName = "Mentor Name" }
        };

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);
        _mockEmailService.Setup(s => s.SendEmailAsync(mentorEmail, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false); // Simulate email failure

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            // Assert.That(result.Message, Is.EqualTo("Failed to send notification email.")); // Adjust
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            _mockMentorApplicationRepository.Verify(r => r.Update(It.Is<MentorApplication>(a => a.Status == ApplicationStatus.WaitingInfo)), Times.Once);
            _mockMentorApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Approved" };

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync((MentorApplication?)null);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ApplicationAlreadyProcessed_ReturnsConflict()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Approved" };
        var application = new MentorApplication { Id = applicationId, Status = ApplicationStatus.Approved, Mentor = new User() }; // Already Approved

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        MentorSubmissionRequest request = new MentorSubmissionRequest(null, null, null, null, null);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, null)).ReturnsAsync((User)null);

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

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, u => u.MentorApplications)).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()));
        _mockMentorApplicationRepository.Setup(x => x.AddAsync(It.IsAny<MentorApplication>()));
        _mockMentorApplicationRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

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

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, u => u.MentorApplications)).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.Update(It.IsAny<User>()));
        _mockMentorApplicationRepository.Setup(x => x.AddAsync(It.IsAny<MentorApplication>()));
        _mockMentorApplicationRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
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

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, u => u.MentorApplications)).ReturnsAsync(user);

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

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, u => u.MentorApplications)).ReturnsAsync(user);

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