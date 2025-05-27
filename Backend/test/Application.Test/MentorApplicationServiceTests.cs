using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplication.Requests;
using Contract.Dtos.MentorApplication.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Application.Test;

public class MentorApplicationServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IMentorApplicationRepository> _mockMentorApplicationRepository;
    private Mock<IEmailService> _mockEmailService;
    private MentorApplicationService _mentorApplicationService;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMentorApplicationRepository = new Mock<IMentorApplicationRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mentorApplicationService = new MentorApplicationService(
            _mockUserRepository.Object,
            _mockMentorApplicationRepository.Object,
            _mockEmailService.Object
        );
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_ReturnsPaginatedListOfApplications()
    {
        // Arrange
        var request = new FilterMentorApplicationRequest { PageIndex = 1, PageSize = 10 };
        var applications = new List<MentorApplication>
        {
            new MentorApplication { Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor One" }, Status = ApplicationStatus.Submitted },
            new MentorApplication { Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor Two" }, Status = ApplicationStatus.Approved }
        }.AsQueryable();

        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            applications.Select(x => new FilterMentorApplicationResponse { MentorName = x.Mentor.FullName }).ToList(),
            applications.Count(),
            request.PageIndex,
            request.PageSize
        );

        _mockMentorApplicationRepository.Setup(r => r.GetAllApplicationsAsync()).Returns(applications);
        _mockMentorApplicationRepository.Setup(r => r.ToPaginatedListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>(), request.PageSize, request.PageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null) Assert.That(result.Value.Items.Count(), Is.EqualTo(2));
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
            new MentorApplication { Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor One" }, Status = ApplicationStatus.Submitted },
            new MentorApplication { Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor Two" }, Status = ApplicationStatus.Approved }
        }.AsQueryable();

        var filteredApplications = applications.Where(x => x.Mentor.FullName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase));
        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            filteredApplications.Select(x => new FilterMentorApplicationResponse { MentorName = x.Mentor.FullName }).ToList(),
            filteredApplications.Count(),
            request.PageIndex,
            request.PageSize
        );

        _mockMentorApplicationRepository.Setup(r => r.GetAllApplicationsAsync()).Returns(applications);
        _mockMentorApplicationRepository.Setup(r => r.ToPaginatedListAsync(It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == 1), request.PageSize, request.PageIndex))
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
        var request = new FilterMentorApplicationRequest { PageIndex = 1, PageSize = 10, Status = ApplicationStatus.Approved };
        var applications = new List<MentorApplication>
        {
            new MentorApplication { Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor One" }, Status = ApplicationStatus.Submitted },
            new MentorApplication { Id = Guid.NewGuid(), Mentor = new User { FullName = "Mentor Two" }, Status = ApplicationStatus.Approved }
        }.AsQueryable();

        var filteredApplications = applications.Where(x => x.Status == request.Status.Value);
        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
             filteredApplications.Select(x => new FilterMentorApplicationResponse { MentorName = x.Mentor.FullName }).ToList(),
            filteredApplications.Count(),
            request.PageIndex,
            request.PageSize
        );

        _mockMentorApplicationRepository.Setup(r => r.GetAllApplicationsAsync()).Returns(applications);
        _mockMentorApplicationRepository.Setup(r => r.ToPaginatedListAsync(It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == 1), request.PageSize, request.PageIndex))
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
        var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync((MentorApplication?)null);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(applicationId, request);

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
        var request = new RequestApplicationInfoRequest { Note = "Please provide more details." };
        var application = new MentorApplication { Id = applicationId, Status = ApplicationStatus.Approved, Mentor = new User() }; // Not Submitted

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(applicationId, request);

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
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(applicationId, request);

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
    public async Task RequestApplicationInfoAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
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
            .ReturnsAsync(true);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null) Assert.That(result.Value.Message, Is.EqualTo("Request for additional information has been sent successfully."));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _mockMentorApplicationRepository.Verify(r => r.Update(It.Is<MentorApplication>(a => a.Status == ApplicationStatus.WaitingInfo && a.Note == request.Note)), Times.Once);
            _mockMentorApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Approved" };

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync((MentorApplication?)null);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(applicationId, request);

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
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Approved" };
        var application = new MentorApplication { Id = applicationId, Status = ApplicationStatus.Approved, Mentor = new User() }; // Already Approved

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var mentorUserId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Well done!" };
        var mentorEmail = "mentor@example.com";
        var mentorUser = new User
        {
            Id = mentorUserId,
            Email = mentorEmail,
            FullName = "Mentor Name",
            Role = new Role { Name = UserRole.Learner }
        };
        var application = new MentorApplication
        {
            Id = applicationId,
            MentorId = mentorUserId,
            Status = ApplicationStatus.Submitted,
            Mentor = mentorUser
        };
        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);
        _mockUserRepository.Setup(r => r.GetByIdAsync(mentorUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(mentorUser);
        _mockEmailService.Setup(s => s.SendEmailAsync(mentorEmail, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            if (result.Value != null) Assert.That(result.Value.Message, Is.EqualTo("Mentor application status updated successfully."));
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            _mockMentorApplicationRepository.Verify(r => r.Update(It.Is<MentorApplication>(a =>
                a.Id == applicationId &&
                a.Status == request.Status &&
                a.Note == request.Note)), Times.Once);
            _mockMentorApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_EmailSendFails_ReturnsInternalServerError()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var mentorUserId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "Approved, but email failed" };
        var mentorEmail = "mentor@example.com";

        var mentorUser = new User
        {
            Id = mentorUserId,
            Email = mentorEmail,
            FullName = "Mentor Name",
            Role = new Role { Name = UserRole.Learner } // Initial role doesn't matter much here, but good to have
        };

        var application = new MentorApplication
        {
            Id = applicationId,
            MentorId = mentorUserId,
            Status = ApplicationStatus.Submitted, // Initial status that allows processing
            Mentor = mentorUser
        };

        _mockMentorApplicationRepository.Setup(r => r.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Simulate email sending failure
        _mockEmailService.Setup(s => s.SendEmailAsync(
                application.Mentor.Email,
                EmailConstants.SUBJECT_MENTOR_APPLICATION_DECISION, // Use the actual subject from your service
                It.Is<string>(body => body.Contains(application.Mentor.FullName) && body.Contains(request.Status.ToString()) && body.Contains(request.Note))))
            .ReturnsAsync(false);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Error, Is.EqualTo("Failed to send notification email.")); // Matches the error message in your service

            // Verify that the application was still updated in the database
            _mockMentorApplicationRepository.Verify(r => r.Update(It.Is<MentorApplication>(a =>
                a.Id == applicationId &&
                a.Status == request.Status &&
                a.Note == request.Note &&
                a.ReviewedAt.HasValue)), Times.Once);
            _mockMentorApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            // Verify email service was called
            _mockEmailService.Verify(s => s.SendEmailAsync(
                application.Mentor.Email,
                EmailConstants.SUBJECT_MENTOR_APPLICATION_DECISION,
                It.Is<string>(body => body.Contains(application.Mentor.FullName) && body.Contains(request.Status.ToString()) && body.Contains(request.Note))), Times.Once);
        });
    }
}