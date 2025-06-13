using Application.Services.MentorApplications;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Application.Test;

[TestFixture]
public class MentorApplicationServiceTestsGetAll
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IMentorApplicationRepository> _mockMentorApplicationRepository;
    private Mock<IEmailService> _mockEmailService;
    private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private Mock<ILogger<MentorApplicationService>> _mockLogger;
    private Mock<HttpRequest> _mockHttpRequest;
    private MentorApplicationService _mentorApplicationService;
    private const int pageSize = 10;
    private const int pageIndex = 1;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMentorApplicationRepository = new Mock<IMentorApplicationRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
        _mockLogger = new Mock<ILogger<MentorApplicationService>>();
        _mockHttpRequest = new Mock<HttpRequest>();

        _mentorApplicationService = new MentorApplicationService(
            _mockUserRepository.Object,
            _mockMentorApplicationRepository.Object,
            _mockEmailService.Object,
            _mockWebHostEnvironment.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_WithoutFilters_ReturnsPaginatedApplications()
    {
        // Arrange
        var mentorApplications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor One",
                    Email = "mentor1@example.com",
                    Bio = "Bio 1",
                    Experiences = "Experience 1",
                    ProfilePhotoUrl = "url1",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 1"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor Two",
                    Email = "mentor2@example.com",
                    Bio = "Bio 2",
                    Experiences = "Experience 2",
                    ProfilePhotoUrl = "url2",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 2"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-1)
            }
        }.AsQueryable();

        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            mentorApplications.Select(x => new FilterMentorApplicationResponse
            {
                MentorApplicationId = x.Id,
                ProfilePhotoUrl = x.Mentor.ProfilePhotoUrl,
                MentorName = x.Mentor.FullName,
                Email = x.Mentor.Email,
                Bio = x.Mentor.Bio,
                Experiences = x.Mentor.Experiences,
                SubmittedAt = x.SubmittedAt,
                Status = x.Status.ToString(),
                Expertises = x.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList()
            }).OrderByDescending(x => x.SubmittedAt).ToList(),
            mentorApplications.Count(),
            pageIndex,
            pageSize
        );

        _mockMentorApplicationRepository.Setup(repo => repo.GetAllApplicationsAsync()).Returns(mentorApplications);
        _mockMentorApplicationRepository.Setup(repo =>
                repo.ToPaginatedListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>(), pageSize, pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var request = new FilterMentorApplicationRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = string.Empty,
            Status = null
        };
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(2));
            Assert.That(result.Value.Items.First().MentorName, Is.EqualTo("Mentor One"));
            _mockMentorApplicationRepository.Verify(repo => repo.GetAllApplicationsAsync(), Times.Once);
            _mockMentorApplicationRepository.Verify(
                repo => repo.ToPaginatedListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>(), pageSize, pageIndex),
                Times.Once);
        });
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_WithKeyword_ReturnsFilteredPaginatedApplications()
    {
        // Arrange
        const string keyword = "Mentor One";
        var mentorApplications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor One",
                    Email = "mentor1@example.com",
                    Bio = "Bio 1",
                    Experiences = "Experience 1",
                    ProfilePhotoUrl = "url1",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 1"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor Two",
                    Email = "mentor2@example.com",
                    Bio = "Bio 2",
                    Experiences = "Experience 2",
                    ProfilePhotoUrl = "url2",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 2"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-1)
            }
        }.AsQueryable();

        var filteredApplicationsQuery = mentorApplications.Where(x => x.Mentor.FullName.Contains(keyword));

        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            filteredApplicationsQuery.Select(x => new FilterMentorApplicationResponse
            {
                MentorApplicationId = x.Id,
                ProfilePhotoUrl = x.Mentor.ProfilePhotoUrl,
                MentorName = x.Mentor.FullName,
                Email = x.Mentor.Email,
                Bio = x.Mentor.Bio,
                Experiences = x.Mentor.Experiences,
                SubmittedAt = x.SubmittedAt,
                Status = x.Status.ToString(),
                Expertises = x.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList()
            }).OrderByDescending(x => x.SubmittedAt).ToList(),
            filteredApplicationsQuery.Count(),
            pageIndex,
            pageSize
        );

        _mockMentorApplicationRepository.Setup(repo => repo.GetAllApplicationsAsync()).Returns(mentorApplications);
        _mockMentorApplicationRepository.Setup(repo =>
                repo.ToPaginatedListAsync(
                    It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == filteredApplicationsQuery.Count()),
                    pageSize,
                    pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var request = new FilterMentorApplicationRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = keyword,
            Status = null
        };
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().MentorName, Is.EqualTo("Mentor One"));
            _mockMentorApplicationRepository.Verify(repo => repo.GetAllApplicationsAsync(), Times.Once);
            _mockMentorApplicationRepository.Verify(
                repo => repo.ToPaginatedListAsync(
                    It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == filteredApplicationsQuery.Count()),
                    pageSize,
                    pageIndex),
                Times.Once);
        });
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_WithStatusFilter_ReturnsFilteredPaginatedApplications()
    {
        // Arrange
        const ApplicationStatus statusFilter = ApplicationStatus.Submitted;
        var mentorApplications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor One",
                    Email = "mentor1@example.com",
                    Bio = "Bio 1",
                    Experiences = "Experience 1",
                    ProfilePhotoUrl = "url1",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 1"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor Two",
                    Email = "mentor2@example.com",
                    Bio = "Bio 2",
                    Experiences = "Experience 2",
                    ProfilePhotoUrl = "url2",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 2"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-1)
            }
        }.AsQueryable();

        var filteredApplicationsQuery = mentorApplications.Where(x => x.Status == statusFilter);

        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            filteredApplicationsQuery.Select(x => new FilterMentorApplicationResponse
            {
                MentorApplicationId = x.Id,
                ProfilePhotoUrl = x.Mentor.ProfilePhotoUrl,
                MentorName = x.Mentor.FullName,
                Email = x.Mentor.Email,
                Bio = x.Mentor.Bio,
                Experiences = x.Mentor.Experiences,
                SubmittedAt = x.SubmittedAt,
                Status = x.Status.ToString(),
                Expertises = x.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList()
            }).OrderByDescending(x => x.SubmittedAt).ToList(),
            filteredApplicationsQuery.Count(),
            pageIndex,
            pageSize
        );

        _mockMentorApplicationRepository.Setup(repo => repo.GetAllApplicationsAsync()).Returns(mentorApplications);
        _mockMentorApplicationRepository.Setup(repo =>
                repo.ToPaginatedListAsync(
                    It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == filteredApplicationsQuery.Count()),
                    pageSize,
                    pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var request = new FilterMentorApplicationRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = string.Empty,
            Status = statusFilter
        };
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items.First().MentorName, Is.EqualTo("Mentor One"));
            Assert.That(result.Value.Items.First().Status, Is.EqualTo(ApplicationStatus.Submitted.ToString()));
            _mockMentorApplicationRepository.Verify(repo => repo.GetAllApplicationsAsync(), Times.Once);
            _mockMentorApplicationRepository.Verify(
                repo => repo.ToPaginatedListAsync(
                    It.Is<IQueryable<FilterMentorApplicationResponse>>(q => q.Count() == filteredApplicationsQuery.Count()),
                    pageSize,
                    pageIndex),
                Times.Once);
        });
    }

    [Test]
    public async Task GetAllMentorApplicationsAsync_WithInvalidStatus_ReturnsAllApplications()
    {
        // Arrange
        const ApplicationStatus invalidStatus = (ApplicationStatus)999;
        var mentorApplications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor One",
                    Email = "mentor1@example.com",
                    Bio = "Bio 1",
                    Experiences = "Experience 1",
                    ProfilePhotoUrl = "url1",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 1"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow
            },
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                Mentor = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Mentor Two",
                    Email = "mentor2@example.com",
                    Bio = "Bio 2",
                    Experiences = "Experience 2",
                    ProfilePhotoUrl = "url2",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 2"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-1)
            }
        }.AsQueryable();

        var paginatedList = new PaginatedList<FilterMentorApplicationResponse>(
            mentorApplications.Select(x => new FilterMentorApplicationResponse
            {
                MentorApplicationId = x.Id,
                ProfilePhotoUrl = x.Mentor.ProfilePhotoUrl,
                MentorName = x.Mentor.FullName,
                Email = x.Mentor.Email,
                Bio = x.Mentor.Bio,
                Experiences = x.Mentor.Experiences,
                SubmittedAt = x.SubmittedAt,
                Status = x.Status.ToString(),
                Expertises = x.Mentor.UserExpertises.Select(ue => ue.Expertise.Name).ToList()
            }).OrderByDescending(x => x.SubmittedAt).ToList(),
            mentorApplications.Count(),
            pageIndex,
            pageSize
        );

        _mockMentorApplicationRepository.Setup(repo => repo.GetAllApplicationsAsync()).Returns(mentorApplications);
        _mockMentorApplicationRepository.Setup(repo =>
                repo.ToPaginatedListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>(), pageSize, pageIndex))
            .ReturnsAsync(paginatedList);

        // Act
        var request = new FilterMentorApplicationRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Keyword = string.Empty,
            Status = invalidStatus
        };
        var result = await _mentorApplicationService.GetAllMentorApplicationsAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(2));
            _mockMentorApplicationRepository.Verify(repo => repo.GetAllApplicationsAsync(), Times.Once);
            _mockMentorApplicationRepository.Verify(
                repo => repo.ToPaginatedListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>(), pageSize, pageIndex),
                Times.Once);
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_UserIsLearner_ReturnsForbidden()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Learner } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            Assert.That(result.Error, Is.EqualTo("You do not have permission to view this mentor application."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(It.IsAny<Guid>()), Times.Never);
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Admin } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(default(MentorApplication));

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Mentor application not found."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_MentorNotOwnApplication_ReturnsForbidden()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Mentor } };
        var application = new MentorApplication
        {
            Id = applicationId,
            MentorId = Guid.NewGuid(), // Different mentor ID
            Mentor = new User { FullName = "Mentor One", Email = "mentor1@example.com" }
        };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            Assert.That(result.Error, Is.EqualTo("You do not have permission to view this mentor application."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_ValidRequestAsAdmin_ReturnsApplicationDetails()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Admin } };
        var application = new MentorApplication
        {
            Id = applicationId,
            MentorId = Guid.NewGuid(),
            Mentor = new User
            {
                FullName = "Mentor One",
                Email = "mentor1@example.com",
                UserExpertises = new List<UserExpertise>
                {
                    new UserExpertise
                    {
                        Expertise = new Expertise
                        {
                            Name = "Expertise 1"
                        },
                        UserId = Guid.Empty,
                        ExpertiseId = Guid.Empty
                    }
                }
            },
            Status = ApplicationStatus.Submitted,
            SubmittedAt = DateTime.UtcNow,
            ApplicationDocuments = new List<ApplicationDocument>
            {
                new ApplicationDocument { Id = Guid.NewGuid(), DocumentType = FileType.Pdf, DocumentUrl = "doc1.pdf" }
            }
        };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.MentorApplicationId, Is.EqualTo(applicationId));
            Assert.That(result.Value.MentorName, Is.EqualTo("Mentor One"));
            Assert.That(result.Value.Email, Is.EqualTo("mentor1@example.com"));
            Assert.That(result.Value.Documents, Has.Count.EqualTo(1));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
        });
    }

    [Test]
    public async Task GetMentorApplicationByIdAsync_ValidRequestAsMentor_ReturnsApplicationDetails()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Mentor } };
        var application = new MentorApplication
        {
            Id = applicationId,
            MentorId = currentUserId, // Same as current user
            Mentor = new User
            {
                FullName = "Mentor One",
                Email = "mentor1@example.com",
                Bio = "Bio 1",
                Experiences = "Experience 1",
                ProfilePhotoUrl = "url1",
                UserExpertises = new List<UserExpertise>
                {
                    new UserExpertise
                    {
                        Expertise = new Expertise
                        {
                            Name = "Expertise 1"
                        },
                        UserId = Guid.Empty,
                        ExpertiseId = Guid.Empty
                    }
                }
            },
            Statement = "Statement",
            Certifications = "Certifications",
            Education = "Education",
            Status = ApplicationStatus.Submitted,
            SubmittedAt = DateTime.UtcNow,
            ReviewedAt = DateTime.UtcNow.AddHours(-1),
            Note = "Note",
            Admin = new User { FullName = "Admin One" },
            ApplicationDocuments = new List<ApplicationDocument>
            {
                new ApplicationDocument { Id = Guid.NewGuid(), DocumentType = FileType.Pdf, DocumentUrl = "doc1.pdf" }
            }
        };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.GetMentorApplicationByIdAsync(currentUserId, applicationId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.MentorApplicationId, Is.EqualTo(applicationId));
            Assert.That(result.Value.MentorName, Is.EqualTo("Mentor One"));
            Assert.That(result.Value.Email, Is.EqualTo("mentor1@example.com"));
            Assert.That(result.Value.Expertises, Contains.Item("Expertise 1"));
            Assert.That(result.Value.Documents, Has.Count.EqualTo(1));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
        });
    }

    [Test]
    public async Task GetListMentorApplicationByMentorIdAsync_UserIsLearner_ReturnsForbidden()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Learner } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.GetListMentorApplicationByMentorIdAsync(currentUserId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            Assert.That(result.Error, Is.EqualTo("You do not have permission to view this mentor application."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByMentorIdAsync(It.IsAny<Guid>()), Times.Never);
        });
    }

    [Test]
    public async Task GetListMentorApplicationByMentorIdAsync_ValidRequest_ReturnsApplications()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var user = new User { Id = currentUserId, Role = new Role { Name = UserRole.Mentor } };
        var applications = new List<MentorApplication>
        {
            new MentorApplication
            {
                Id = Guid.NewGuid(),
                MentorId = currentUserId,
                Mentor = new User
                {
                    FullName = "Mentor One",
                    Email = "mentor1@example.com",
                    Bio = "Bio 1",
                    Experiences = "Experience 1",
                    ProfilePhotoUrl = "url1",
                    UserExpertises = new List<UserExpertise>
                    {
                        new UserExpertise
                        {
                            Expertise = new Expertise
                            {
                                Name = "Expertise 1"
                            },
                            UserId = Guid.Empty,
                            ExpertiseId = Guid.Empty
                        }
                    }
                },
                Status = ApplicationStatus.Submitted,
                SubmittedAt = DateTime.UtcNow
            }
        }.AsQueryable();
        var responseList = applications.Select(application => new FilterMentorApplicationResponse
        {
            MentorApplicationId = application.Id,
            ProfilePhotoUrl = application.Mentor.ProfilePhotoUrl,
            MentorName = application.Mentor.FullName,
            Email = application.Mentor.Email,
            Bio = application.Mentor.Bio,
            Experiences = application.Mentor.Experiences,
            Expertises = application.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList(),
            Status = application.Status.ToString(),
            SubmittedAt = application.SubmittedAt
        }).OrderByDescending(application => application.SubmittedAt).ToList();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByMentorIdAsync(currentUserId))
            .Returns(applications);
        _mockMentorApplicationRepository.Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>()))
            .ReturnsAsync(responseList);

        // Act
        var result = await _mentorApplicationService.GetListMentorApplicationByMentorIdAsync(currentUserId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!, Has.Count.EqualTo(1));
            Assert.That(result.Value!.First().MentorName, Is.EqualTo("Mentor One"));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(currentUserId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByMentorIdAsync(currentUserId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.ToListAsync(It.IsAny<IQueryable<FilterMentorApplicationResponse>>()), Times.Once);
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Additional info needed" };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(default(MentorApplication));

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Mentor application not found."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_ApplicationNotSubmitted_ReturnsConflict()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Additional info needed" };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.WaitingInfo,
            Mentor = new User { Email = "mentor@example.com", FullName = "Mentor One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(result.Error, Is.EqualTo("You can only request additional information for submitted applications."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_EmailFails_ReturnsInternalServerError()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Additional info needed" };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted,
            Mentor = new User { Email = "mentor@example.com", FullName = "Mentor One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Error, Is.EqualTo("Failed to send notification email."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(application), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(application.Mentor.Email, EmailConstants.SUBJECT_REQUEST_APPLICATION_INFO, It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task RequestApplicationInfoAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new RequestApplicationInfoRequest { Note = "Additional info needed" };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted,
            Mentor = new User { Email = "mentor@example.com", FullName = "Mentor One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _mentorApplicationService.RequestApplicationInfoAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value!.Message, Is.EqualTo("Request for additional information has been sent successfully."));
            Assert.That(application.Status, Is.EqualTo(ApplicationStatus.WaitingInfo));
            Assert.That(application.Note, Is.EqualTo(request.Note));
            Assert.That(application.AdminId, Is.EqualTo(adminId));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(application), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(application.Mentor.Email, EmailConstants.SUBJECT_REQUEST_APPLICATION_INFO, It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "ApprovedApplications" };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(default(MentorApplication));

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Mentor application not found."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ApplicationAlreadyApproved_ReturnsConflict()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "ApprovedApplications" };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Approved,
            Mentor = new User { Email = "mentor@example.com", FullName = "Mentor One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(result.Error, Is.EqualTo("Application is already approved or rejected."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_EmailFails_ReturnsInternalServerError()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "ApprovedApplications" };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted,
            Mentor = new User { Email = "mentor@example.com", FullName = "Mentor One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Error, Is.EqualTo("Failed to send notification email."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(application), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(application.Mentor.Email, EmailConstants.SUBJECT_MENTOR_APPLICATION_DECISION, It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task UpdateApplicationStatusAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var request = new UpdateApplicationStatusRequest { Status = ApplicationStatus.Approved, Note = "ApprovedApplications" };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted,
            Mentor = new User { Email = "mentor@example.com", FullName = "Mentor One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _mentorApplicationService.UpdateApplicationStatusAsync(adminId, applicationId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value!.Message, Is.EqualTo("Mentor application status updated successfully."));
            Assert.That(application.Status, Is.EqualTo(request.Status));
            Assert.That(application.Note, Is.EqualTo(request.Note));
            Assert.That(application.AdminId, Is.EqualTo(adminId));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationByIdAsync(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(application), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(application.Mentor.Email, EmailConstants.SUBJECT_MENTOR_APPLICATION_DECISION, It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task EditMentorApplicationAsync_ApplicationNotFound_ReturnsNotFound()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var request = new UpdateMentorApplicationRequest
        {
            WorkExperience = "Updated Experience",
            Certifications = "Updated Certs",
            Education = "Updated Education",
            Statement = "Updated Statement"
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationsToUpdate(applicationId))
            .ReturnsAsync(default(MentorApplication));

        // Act
        var result = await _mentorApplicationService.EditMentorApplicationAsync(applicationId, request, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Mentor application not found."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationsToUpdate(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task EditMentorApplicationAsync_ApplicationNotWaitingInfo_ReturnsBadRequest()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var request = new UpdateMentorApplicationRequest
        {
            WorkExperience = "Updated Experience",
            Certifications = "Updated Certs",
            Education = "Updated Education",
            Statement = "Updated Statement"
        };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted,
            Mentor = new User { Email = "mentor@example.com" },
            Admin = new User { Email = "admin@example.com", FullName = "Admin One" }
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationsToUpdate(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.EditMentorApplicationAsync(applicationId, request, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(result.Error, Is.EqualTo("You can only update applications when the status is WaitingInfo."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationsToUpdate(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task EditMentorApplicationAsync_DuplicateFiles_ReturnsBadRequest()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var request = new UpdateMentorApplicationRequest
        {
            WorkExperience = "Updated Experience",
            Certifications = "Updated Certs",
            Education = "Updated Education",
            Statement = "Updated Statement",
            Documents = new List<IFormFile>
            {
                new Mock<IFormFile>().Object,
                new Mock<IFormFile>().Object
            }
        };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.WaitingInfo,
            Mentor = new User { Id = Guid.NewGuid(), Email = "mentor@example.com" },
            Admin = new User { Email = "admin@example.com", FullName = "Admin One" },
            ApplicationDocuments = new List<ApplicationDocument>()
        };
        var mockFile1 = new Mock<IFormFile>();
        var mockFile2 = new Mock<IFormFile>();
        mockFile1.Setup(f => f.FileName).Returns("doc.pdf");
        mockFile2.Setup(f => f.FileName).Returns("doc.pdf");
        request.Documents = new List<IFormFile> { mockFile1.Object, mockFile2.Object };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationsToUpdate(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _mentorApplicationService.EditMentorApplicationAsync(applicationId, request, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("Duplicate files detected in request: doc.pdf"));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationsToUpdate(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        });
    }

    [Test]
    public async Task EditMentorApplicationAsync_EmailFails_ReturnsInternalServerError()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();
        var request = new UpdateMentorApplicationRequest
        {
            WorkExperience = "Updated Experience",
            Certifications = "Updated Certs",
            Education = "Updated Education",
            Statement = "Updated Statement",
            Documents = new List<IFormFile>()
        };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.WaitingInfo,
            Mentor = new User { Id = mentorId, Email = "mentor@example.com" },
            Admin = new User { Email = "admin@example.com", FullName = "Admin One" },
            ApplicationDocuments = new List<ApplicationDocument>()
        };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationsToUpdate(applicationId))
            .ReturnsAsync(application);
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _mentorApplicationService.EditMentorApplicationAsync(applicationId, request, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Error, Is.EqualTo("Failed to send notification email."));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationsToUpdate(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(application), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(application.Admin.Email, EmailConstants.SUBJECT_UPDATE_APPLICATION, It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task EditMentorApplicationAsync_ValidRequestWithDocuments_ReturnsSuccess()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();
        var request = new UpdateMentorApplicationRequest
        {
            WorkExperience = "Updated Experience",
            Certifications = "Updated Certs",
            Education = "Updated Education",
            Statement = "Updated Statement",
            Documents = new List<IFormFile>()
        };
        var application = new MentorApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.WaitingInfo,
            Mentor = new User { Id = mentorId, Email = "mentor@example.com" },
            Admin = new User { Email = "admin@example.com", FullName = "Admin One" },
            ApplicationDocuments = new List<ApplicationDocument>
            {
                new ApplicationDocument { Id = Guid.NewGuid(), DocumentType = FileType.Pdf, DocumentUrl = "/documents/old.pdf" }
            }
        };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("new.pdf");
        mockFile.Setup(f => f.Length).Returns(1000);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        request.Documents = new List<IFormFile> { mockFile.Object };
        _mockMentorApplicationRepository.Setup(repo => repo.GetMentorApplicationsToUpdate(applicationId))
            .ReturnsAsync(application);
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("wwwroot");

        // Act
        var result = await _mentorApplicationService.EditMentorApplicationAsync(applicationId, request, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.True);
            Assert.That(application.Mentor.Experiences, Is.EqualTo(request.WorkExperience));
            Assert.That(application.Certifications, Is.EqualTo(request.Certifications));
            Assert.That(application.Education, Is.EqualTo(request.Education));
            Assert.That(application.Status, Is.EqualTo(ApplicationStatus.Submitted));
            Assert.That(application.ApplicationDocuments.Count, Is.EqualTo(1));
            Assert.That(application.ApplicationDocuments.First().DocumentUrl, Contains.Substring("new.pdf"));
            _mockMentorApplicationRepository.Verify(repo => repo.GetMentorApplicationsToUpdate(applicationId), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.Update(application), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(application.Admin.Email, EmailConstants.SUBJECT_UPDATE_APPLICATION, It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var submission = new MentorSubmissionRequest(
            WorkExperience: "Experience",
            Statement: "Statement",
            Certifications: "Certs",
            Education: "Education",
            Documents: new List<IFormFile>()
        );
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(default(User));

        // Act
        var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, submission, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("User not found"));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_ActiveApplicationExists_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var submission = new MentorSubmissionRequest(
            WorkExperience: "Experience",
            Statement: "Statement",
            Certifications: "Certs",
            Education: "Education",
            Documents: new List<IFormFile>()
        );
        var user = new User
        {
            Id = userId,
            MentorApplications = new List<MentorApplication>
            {
                new MentorApplication { Status = ApplicationStatus.Submitted }
            }
        };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, submission, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(result.Error, Is.EqualTo("User has an active or pending mentor application."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_InvalidFileContentType_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var submission = new MentorSubmissionRequest(
            WorkExperience: "Experience",
            Statement: "Statement",
            Certifications: "Certs",
            Education: "Education",
            Documents: new List<IFormFile>()
        );
        var user = new User { Id = userId, MentorApplications = new List<MentorApplication>() };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("invalid.exe");
        mockFile.Setup(f => f.Length).Returns(1000);
        mockFile.Setup(f => f.ContentType).Returns("application/octet-stream");
        submission = submission with { Documents = new List<IFormFile> { mockFile.Object } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, submission, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("File content type is not allowed."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_FileSizeTooLarge_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var submission = new MentorSubmissionRequest(
            WorkExperience: "Experience",
            Statement: "Statement",
            Certifications: "Certs",
            Education: "Education",
            Documents: new List<IFormFile>()
        );
        var user = new User { Id = userId, MentorApplications = new List<MentorApplication>() };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("doc.pdf");
        mockFile.Setup(f => f.Length).Returns(FileConstants.MAX_FILE_SIZE + 1);
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");
        submission = submission with { Documents = new List<IFormFile> { mockFile.Object } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, submission, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Error, Is.EqualTo("File size must not exceed 1MB."));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_FileSaveFails_ReturnsInternalServerError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var submission = new MentorSubmissionRequest(
            WorkExperience: "Experience",
            Statement: "Statement",
            Certifications: "Certs",
            Education: "Education",
            Documents: new List<IFormFile>()
        );
        var user = new User { Id = userId, MentorApplications = new List<MentorApplication>() };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("doc.pdf");
        mockFile.Setup(f => f.Length).Returns(1000);
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("File save error"));
        submission = submission with { Documents = new List<IFormFile> { mockFile.Object } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, submission, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Error, Contains.Substring("Failed to save file"));
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorApplication>()), Times.Never);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        });
    }

    [Test]
    public async Task CreateMentorApplicationAsync_ValidRequestWithDocuments_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var submission = new MentorSubmissionRequest(
            WorkExperience: "Experience",
            Statement: "Statement",
            Certifications: "Certs",
            Education: "Education",
            Documents: new List<IFormFile>()
        );
        var user = new User { Id = userId, MentorApplications = new List<MentorApplication>() };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("doc.pdf");
        mockFile.Setup(f => f.Length).Returns(1000);
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        submission = submission with { Documents = new List<IFormFile> { mockFile.Object } };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _mockMentorApplicationRepository.Setup(repo => repo.AddAsync(It.IsAny<MentorApplication>()))
            .Callback<MentorApplication>(app => app.Id = Guid.NewGuid());
        _mockMentorApplicationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _mentorApplicationService.CreateMentorApplicationAsync(userId, submission, _mockHttpRequest.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.True);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.AddAsync(It.IsAny<MentorApplication>()), Times.Once);
            _mockMentorApplicationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }
}