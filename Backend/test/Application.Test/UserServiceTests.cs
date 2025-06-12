using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
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

namespace Application.Test
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IWebHostEnvironment> _mockWebHostService;
        private Mock<ILogger<UserService>> _mockLogger;
        private UserService _userService;
        private string _tempFolder;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _mockWebHostService = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _emailServiceMock.Object, _mockWebHostService.Object, _mockLogger.Object); _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _mockWebHostService.Setup(e => e.WebRootPath).Returns(_tempFolder);
            Directory.CreateDirectory(_tempFolder);
        }

        private IFormFile CreateMockFormFile(string fileName, string contentType, long size)
        {
            var content = new byte[size];
            new Random().NextBytes(content);
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Test]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FullName = "Test User", Email = "test@example.com", Role = new Role { Id = 1, Name = UserRole.Learner } };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value?.Id, Is.EqualTo(userId));
                Assert.That(result.Value?.FullName, Is.EqualTo("Test User"));
                Assert.That(result.Value?.Role, Is.EqualTo(UserRole.Learner.ToString()));
            });
        }

        [Test]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with id {userId} not found."));
            });
        }

        [Test]
        public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
        {
            // Arrange
            const string email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Test User",
                Email = email,
                Role = new Role { Id = 1, Name = UserRole.Learner }
            };
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value?.Id, Is.EqualTo(user.Id));
                Assert.That(result.Value?.FullName, Is.EqualTo("Test User"));
                Assert.That(result.Value?.Role, Is.EqualTo(UserRole.Learner.ToString()));
                _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            });
        }

        [Test]
        public async Task GetUserByEmailAsync_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            const string email = "nonexistent@example.com";
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo("User not found"));
                _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
            });
        }

        [Test]
        public async Task FilterUserAsync_ReturnsFilteredUsers()
        {
            // Arrange
            var request = new UserFilterPagedRequest { PageIndex = 1, PageSize = 10, FullName = "Test", RoleName = "Learner" };
            var users = new List<User>
            {
                new User {
                    Id = Guid.NewGuid(),
                    FullName = "Test User 1",
                    Email = "test1@example.com",
                    Status = UserStatus.Active,
                    JoinedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
                    LastActive = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                    Role = new Role { Id = 1, Name = UserRole.Learner }
                },
                new User {
                    Id = Guid.NewGuid(),
                    FullName = "Another Test User",
                    Email = "test2@example.com",
                    Status = UserStatus.Active,
                    JoinedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-20)),
                    LastActive = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                    Role = new Role { Id = 2, Name = UserRole.Learner }
                }
            };

            var queryableUsers = users.AsQueryable();

            _mockUserRepository.Setup(repo => repo.GetAll())
                .Returns(queryableUsers);

            var filteredQueryable = queryableUsers
                .Where(user => user.FullName.Contains(request.FullName))
                .Where(user => user.Role.Name.ToString().Equals(request.RoleName))
                .Select(u => new GetUserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.Name.ToString(),
                    Status = u.Status.ToString(),
                    JoinedDate = u.JoinedDate,
                    LastActive = u.LastActive
                });

            var paginatedUserResponses = new PaginatedList<GetUserResponse>(
                filteredQueryable.ToList(),
                filteredQueryable.Count(),
                request.PageIndex,
                request.PageSize
            );

            _mockUserRepository.Setup(repo => repo.ToPaginatedListAsync(
                    It.IsAny<IQueryable<GetUserResponse>>(),
                    request.PageSize,
                    request.PageIndex))
                .ReturnsAsync(paginatedUserResponses);

            // Act
            var result = await _userService.FilterUserAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value?.Items.Count, Is.EqualTo(filteredQueryable.Count()));
                Assert.That(result.Value?.TotalCount, Is.EqualTo(filteredQueryable.Count()));

                _mockUserRepository.Verify(repo => repo.GetAll(), Times.Once);
                _mockUserRepository.Verify(repo => repo.ToPaginatedListAsync(
                    It.IsAny<IQueryable<GetUserResponse>>(),
                    request.PageSize,
                    request.PageIndex), Times.Once);
            });
        }

        [Test]
        public async Task FilterUserAsync_NoUsersFound_ReturnsEmptyList()
        {
            // Arrange
            var request = new UserFilterPagedRequest { PageIndex = 1, PageSize = 10, FullName = "NonexistentUser" };

            _mockUserRepository.Setup(repo => repo.GetAll())
                .Returns(new List<User>().AsQueryable);

            var emptyUserResponses = new List<GetUserResponse>();
            var emptyPaginatedList = new PaginatedList<GetUserResponse>(
                emptyUserResponses,
                0,
                request.PageIndex,
                request.PageSize
            );

            _mockUserRepository.Setup(repo => repo.ToPaginatedListAsync(
                    It.IsAny<IQueryable<GetUserResponse>>(),
                    request.PageSize,
                    request.PageIndex))
                .ReturnsAsync(emptyPaginatedList);

            // Act
            var result = await _userService.FilterUserAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value?.Items.Count, Is.EqualTo(0));
                Assert.That(result.Value?.TotalCount, Is.EqualTo(0));

                _mockUserRepository.Verify(repo => repo.GetAll(), Times.Once);
                _mockUserRepository.Verify(repo => repo.ToPaginatedListAsync(
                    It.IsAny<IQueryable<GetUserResponse>>(),
                    request.PageSize,
                    request.PageIndex), Times.Once);
            });
        }

        [Test]
        public async Task EditUserDetailAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserProfileRequest(
                "Updated Name",
                1,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                SessionFrequency.Weekly,
                60,
                LearningStyle.Visual,
                false,
                false,
                false,
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() }
            );
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.EditUserDetailAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with ID {userId} not found."));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            });
        }

        [Test]
        public async Task EditUserDetailAsync_InvalidAvailabilityIds_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserProfileRequest(
                "Updated Name",
                1,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                SessionFrequency.Weekly,
                60,
                LearningStyle.Visual,
                false,
                false,
                false,
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() }
            );
            var user = new User { Id = userId };
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Availability, Guid>(request.AvailabilityIds!))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.EditUserDetailAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Invalid Availability IDs"));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<Availability, Guid>(request.AvailabilityIds!), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            });
        }

        [Test]
        public async Task EditUserDetailAsync_InvalidExpertiseIds_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserProfileRequest(
                "Updated Name",
                1,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                SessionFrequency.Weekly,
                60,
                LearningStyle.Visual,
                false,
                false,
                false,
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() }
            );
            var user = new User { Id = userId };
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Availability, Guid>(request.AvailabilityIds!))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Expertise, Guid>(request.ExpertiseIds!))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.EditUserDetailAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Invalid Expertise IDs"));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<Expertise, Guid>(request.ExpertiseIds!), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            });
        }

        [Test]
        public async Task EditUserDetailAsync_InvalidTeachingApproachIds_ReturnsBadRequest()
        {
            // Av
            var userId = Guid.NewGuid();
            var request = new EditUserProfileRequest(
                "Updated Name",
                1,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                SessionFrequency.Weekly,
                60,
                LearningStyle.Visual,
                false,
                false,
                false,
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() }
            );
            var user = new User { Id = userId };
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Availability, Guid>(request.AvailabilityIds!))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Expertise, Guid>(request.ExpertiseIds!))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<TeachingApproach, Guid>(request.TeachingApproachIds!))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.EditUserDetailAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Invalid Teaching Approach IDs"));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<TeachingApproach, Guid>(request.TeachingApproachIds!), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            });
        }

        [Test]
        public async Task EditUserDetailAsync_InvalidCategoryIds_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserProfileRequest(
                "Updated Name",
                1,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                SessionFrequency.Weekly,
                60,
                LearningStyle.Visual,
                false,
                false,
                false,
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() }
            );
            var user = new User { Id = userId };
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Availability, Guid>(request.AvailabilityIds!))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Expertise, Guid>(request.ExpertiseIds!))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<TeachingApproach, Guid>(request.TeachingApproachIds!))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Category, Guid>(request.CategoryIds!))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.EditUserDetailAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Invalid Category IDs"));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<Category, Guid>(request.CategoryIds!), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            });
        }

        [Test]
        public async Task EditUserDetailAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserProfileRequest(
                "Updated Name",
                1,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                SessionFrequency.Weekly,
                60,
                LearningStyle.Visual,
                false,
                false,
                false,
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid() }, 
                new List<Guid> { Guid.NewGuid() }
            );
            var user = new User
            {
                Id = userId,
                UserAvailabilities = new List<UserAvailability>(),
                UserExpertises = new List<UserExpertise>(),
                UserCategories = new List<UserCategory>(),
                UserTeachingApproaches = new List<UserTeachingApproach>()
            };
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Availability, Guid>(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Expertise, Guid>(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<Category, Guid>(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.CheckEntityListExist<TeachingApproach, Guid>(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            _mockUserRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.EditUserDetailAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(user.FullName, Is.EqualTo(request.FullName));
                Assert.That(user.UserAvailabilities, Has.Count.EqualTo(1));
                Assert.That(user.UserExpertises, Has.Count.EqualTo(1));
                Assert.That(user.UserCategories, Has.Count.EqualTo(1));
                Assert.That(user.UserTeachingApproaches, Has.Count.EqualTo(1));
                Assert.That(user.Status, Is.EqualTo(UserStatus.Active));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<Availability, Guid>(It.IsAny<List<Guid>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<Expertise, Guid>(It.IsAny<List<Guid>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<Category, Guid>(It.IsAny<List<Guid>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.CheckEntityListExist<TeachingApproach, Guid>(It.IsAny<List<Guid>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            });
        }

        [Test]
        public async Task EditUserAsync_UserExists_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserRequest { FullName = "Updated Name", Email = "updated@example.com", Role = "Admin" };
            var user = new User { Id = userId, FullName = "Old Name", Email = "old@example.com", RoleId = 2 };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            _mockUserRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.EditUserAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.True);
                _mockUserRepository.Verify(repo => repo.Update(It.Is<User>(u =>
                    u.FullName == request.FullName &&
                    u.Email == request.Email &&
                    u.RoleId == (int)Enum.Parse(typeof(UserRole), request.Role))), Times.Once);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            });
        }

        [Test]
        public async Task EditUserAsync_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new EditUserRequest { FullName = "Updated Name", Email = "updated@example.com", Role = "Admin" };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.EditUserAsync(userId, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with id {userId} not found."));
            });
        }

        [Test]
        public async Task ChangeUserStatusAsync_UserExistsAndActive_DeactivatesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Status = UserStatus.Active };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            _mockUserRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.ChangeUserStatusAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.True);
                Assert.That(user.Status, Is.EqualTo(UserStatus.Deactivated));
                _mockUserRepository.Verify(repo => repo.Update(It.Is<User>(u => u.Status == UserStatus.Deactivated)), Times.Once);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            });
        }

        [Test]
        public async Task ChangeUserStatusAsync_UserExistsAndDeactivated_ActivatesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Status = UserStatus.Deactivated };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            _mockUserRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.ChangeUserStatusAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.True);
                Assert.That(user.Status, Is.EqualTo(UserStatus.Active));
                _mockUserRepository.Verify(repo => repo.Update(It.Is<User>(u => u.Status == UserStatus.Active)), Times.Once);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            });
        }

        [Test]
        public async Task ChangeUserStatusAsync_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.ChangeUserStatusAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with id {userId} not found."));
            });
        }

        [Test]
        public async Task UploadAvatarAsync_WhenFileIsNull_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IFormFile file = null!;

            // Act
            var result = await _userService.UploadAvatarAsync(userId, null, file);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File not selected"));
            });

        }

        [Test]
        public async Task UploadAvatarAsync_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);

            _mockUserRepository
                .Setup(r => r.GetByIdAsync(userId, null))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _userService.UploadAvatarAsync(userId, null!, mockFile.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with ID {userId} not found"));
            });
        }

        [Test]
        public async Task UploadAvatarAsync_WithValidFile_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var file = CreateMockFormFile("avatar.jpg", "image/jpeg", 500 * 1024);
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Scheme).Returns("http");
            requestMock.Setup(r => r.Host).Returns(new HostString("localhost"));

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, null)).ReturnsAsync(new User { Id = userId });

            var result = await _userService.UploadAvatarAsync(userId, requestMock.Object, file);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Does.Contain($"/images/{userId}"));
            });
        }

        [Test]
        public async Task UploadAvatarAsync_InvalidContentType_ReturnsBadRequest()
        {
            var file = CreateMockFormFile("file.jpg", "application/octet-stream", 1000);
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
            .ReturnsAsync(new User { Id = Guid.NewGuid() });

            var result = await _userService.UploadAvatarAsync(Guid.NewGuid(), Mock.Of<HttpRequest>(), file);


            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File content type is not allowed."));
            });

        }

        [Test]
        public async Task UploadAvatarAsync_TooLarge_ReturnsBadRequest()
        {
            var file = CreateMockFormFile("file.jpg", "image/jpeg", FileConstants.MAX_FILE_SIZE + 1);
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
            .ReturnsAsync(new User { Id = Guid.NewGuid() });

            var result = await _userService.UploadAvatarAsync(Guid.NewGuid(), Mock.Of<HttpRequest>(), file);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File size must not exceed 1MB."));
            });

        }

        [Test]
        public async Task UploadAvatarAsync_SaveFails_ReturnsInternalServerError()
        {
            var userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("avatar.jpg");
            fileMock.Setup(f => f.Length).Returns(1000);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).ThrowsAsync(new IOException("Disk full"));

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
            .ReturnsAsync(new User { Id = userId });

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Scheme).Returns("http");
            requestMock.Setup(r => r.Host).Returns(new HostString("localhost"));

            var result = await _userService.UploadAvatarAsync(userId, requestMock.Object, fileMock.Object);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(result.Error, Does.Contain("Failed to save file"));
            });
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void RemoveAvatar_ImageUrlIsNullOrWhiteSpace_ReturnsBadRequest(string? imageUrl)
        {
            // Arrange
            var service = new UserService(null!, null!, _mockWebHostService.Object, _mockLogger.Object);

            // Act
            var result = service.RemoveAvatar(imageUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Image URL is required."));
            });
        }

        [TestCase("not-a-valid-url")]
        [TestCase("ftp://invalid.com/image.png")]
        [TestCase("file://localhost/image.jpg")]
        public void RemoveAvatar_InvalidUrlFormat_ReturnsBadRequest(string imageUrl)
        {
            // Arrange
            var service = new UserService(null!, null!, _mockWebHostService.Object, _mockLogger.Object);

            // Act
            var result = service.RemoveAvatar(imageUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Invalid image URL format."));
            });
        }

        [Test]
        public void RemoveAvatar_FileDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var imageUrl = "http://localhost/images/avatar.jpg";
            var service = new UserService(null!, null!, _mockWebHostService.Object, _mockLogger.Object);
            var filePath = Path.Combine(_mockWebHostService.Object.WebRootPath, "images", "avatar.jpg");

            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Ensure file doesn't exist
            }

            // Act
            var result = service.RemoveAvatar(imageUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo("Avatar file not found."));
            });
        }

        [Test]
        public void RemoveAvatar_FileExists_ReturnsSuccess()
        {
            // Arrange
            var fileName = "test-avatar.jpg";
            var imagesPath = Path.Combine(_mockWebHostService.Object.WebRootPath, "images");
            var filePath = Path.Combine(imagesPath, fileName);
            Directory.CreateDirectory(imagesPath);
            File.WriteAllText(filePath, "dummy data");

            var imageUrl = $"http://localhost/images/{fileName}";
            var service = new UserService(null!, null!, _mockWebHostService.Object, _mockLogger.Object);

            // Act
            var result = service.RemoveAvatar(imageUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(File.Exists(filePath), Is.False);
            });
        }

        [Test]
        public async Task ForgotPasswordRequest_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            const string email = "nonexistent@example.com";
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.ForgotPasswordRequest(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo("User not found"));
                _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
                _emailServiceMock.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            });
        }

        [Test]
        public async Task ForgotPasswordRequest_EmailSendFails_ReturnsInternalServerError()
        {
            // Arrange
            const string email = "test@example.com";
            var user = new User { Id = Guid.NewGuid(), Email = email, Role = new Role { Name = UserRole.Learner } };
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            _mockUserRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _emailServiceMock.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.ForgotPasswordRequest(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(result.Error, Is.EqualTo("Failed to send email"));
                _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
                _emailServiceMock.Verify(service => service.SendEmailAsync(email, EmailConstants.SUBJECT_RESET_PASSWORD, It.IsAny<string>()), Times.Once);
            });
        }

        [Test]
        public async Task ForgotPasswordRequest_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            const string email = "test@example.com";
            var user = new User { Id = Guid.NewGuid(), Email = email, Role = new Role { Name = UserRole.Learner } };
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            _mockUserRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _emailServiceMock.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.ForgotPasswordRequest(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(user.PasswordHash, Is.Not.Null);
                _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email, It.IsAny<Expression<Func<User, object>>>()), Times.Once);
                _mockUserRepository.Verify(repo => repo.Update(user), Times.Once);
                _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
                _emailServiceMock.Verify(service => service.SendEmailAsync(email, EmailConstants.SUBJECT_RESET_PASSWORD, It.IsAny<string>()), Times.Once);
            });
        }

        [Test]
        public async Task GetUserDetailAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserDetailAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo("User not found"));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
            });
        }

        [Test]
        public async Task GetUserDetailAsync_UserExists_ReturnsUserDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FullName = "Test User",
                Email = "test@example.com",
                Role = new Role { Name = UserRole.Learner },
                UserAvailabilities = new List<UserAvailability> { new UserAvailability
                    {
                        AvailabilityId = Guid.NewGuid(),
                        UserId = Guid.Empty
                    }
                },
                UserExpertises = new List<UserExpertise> { new UserExpertise
                    {
                        ExpertiseId = Guid.NewGuid(),
                        UserId = Guid.Empty
                    }
                }
            };
            _mockUserRepository.Setup(repo => repo.GetUserDetailAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserDetailAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value?.FullName, Is.EqualTo("Test User"));
                Assert.That(result.Value?.AvailabilityIds!, Has.Count.EqualTo(1));
                Assert.That(result.Value?.ExpertiseIds!, Has.Count.EqualTo(1));
                _mockUserRepository.Verify(repo => repo.GetUserDetailAsync(userId), Times.Once);
            });
        }

        [Test]
        public async Task UploadDocumentAsync_FileIsNull_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            IFormFile file = null!;

            // Act
            var result = await _userService.UploadDocumentAsync(userId, null, file);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File not selected"));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), null), Times.Never);
            });
        }

        [Test]
        public async Task UploadDocumentAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var file = CreateMockFormFile("doc.pdf", "application/pdf", 1000);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.UploadDocumentAsync(userId, Mock.Of<HttpRequest>(), file);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with ID {userId} not found"));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task UploadDocumentAsync_InvalidContentType_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var file = CreateMockFormFile("doc.exe", "application/octet-stream", 1000);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.UploadDocumentAsync(userId, Mock.Of<HttpRequest>(), file);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File content type is not allowed."));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task UploadDocumentAsync_FileTooLarge_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var file = CreateMockFormFile("doc.pdf", "application/pdf", FileConstants.MAX_FILE_SIZE + 1);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.UploadDocumentAsync(userId, Mock.Of<HttpRequest>(), file);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("File size must not exceed 1MB."));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task UploadDocumentAsync_SaveFails_ReturnsInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("doc.pdf");
            fileMock.Setup(f => f.Length).Returns(1000);
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).ThrowsAsync(new IOException("Disk full"));
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Scheme).Returns("http");
            requestMock.Setup(r => r.Host).Returns(new HostString("localhost"));

            // Act
            var result = await _userService.UploadDocumentAsync(userId, requestMock.Object, fileMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(result.Error, Contains.Substring("Failed to save file"));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task UploadDocumentAsync_ValidFile_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var file = CreateMockFormFile("doc.pdf", "application/pdf", 500 * 1024);
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Scheme).Returns("http");
            requestMock.Setup(r => r.Host).Returns(new HostString("localhost"));
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.UploadDocumentAsync(userId, requestMock.Object, file);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Contains.Substring($"/documents/{userId}/"));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task RemoveDocumentAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentUrl = "http://localhost/documents/doc.pdf";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.RemoveDocumentAsync(userId, documentUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with ID {userId} not found"));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task RemoveDocumentAsync_DocumentUrlIsNullOrWhiteSpace_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentUrl = "   ";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.RemoveDocumentAsync(userId, documentUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Document URL is required."));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task RemoveDocumentAsync_InvalidUrlFormat_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            const string documentUrl = "not-a-valid-url";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.RemoveDocumentAsync(userId, documentUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.Error, Is.EqualTo("Invalid document URL format."));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task RemoveDocumentAsync_UnauthorizedUserId_ReturnsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var documentUrl = $"http://localhost/documents/{differentUserId}/doc.pdf";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.RemoveDocumentAsync(userId, documentUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(result.Error, Is.EqualTo("You are not allowed to delete this file."));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task RemoveDocumentAsync_FileDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentUrl = $"http://localhost/documents/{userId}/doc.pdf";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });
            var filePath = Path.Combine(_mockWebHostService.Object.WebRootPath, "documents", userId.ToString(), "doc.pdf");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Act
            var result = await _userService.RemoveDocumentAsync(userId, documentUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo("Document file not found."));
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [Test]
        public async Task RemoveDocumentAsync_FileExists_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            const string fileName = "doc.pdf";
            var documentsPath = Path.Combine(_mockWebHostService.Object.WebRootPath, "documents", userId.ToString());
            var filePath = Path.Combine(documentsPath, fileName);
            Directory.CreateDirectory(documentsPath);
            await File.WriteAllTextAsync(filePath, "dummy data");
            var documentUrl = $"http://localhost/documents/{userId}/{fileName}";
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, null))
                .ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _userService.RemoveDocumentAsync(userId, documentUrl);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(result.Value, Is.True);
                Assert.That(File.Exists(filePath), Is.False);
                _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId, null), Times.Once);
            });
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempFolder))
            {
                Directory.Delete(_tempFolder, true);
            }
        }
    }
}