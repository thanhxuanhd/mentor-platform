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
using NUnit.Framework.Internal;
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
        private string _tempImagesFolder;       

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _mockWebHostService = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _emailServiceMock.Object, _mockWebHostService.Object, _mockLogger.Object); _tempImagesFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _mockWebHostService.Setup(e => e.WebRootPath).Returns(_tempImagesFolder);
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _emailServiceMock.Object, _mockWebHostService.Object, _mockLogger.Object);

            Directory.CreateDirectory(_tempImagesFolder);
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
            var emptyUsersList = new List<User>();
            var emptyQueryable = emptyUsersList.AsQueryable();

            _mockUserRepository.Setup(repo => repo.GetAll())
                .Returns(emptyQueryable);

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
            IFormFile file = null;

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
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.UploadAvatarAsync(userId, null, mockFile.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.Error, Is.EqualTo($"User with ID {userId} not found"));
            });
        }

        [Test]
        public async Task UploadAvatar_WithValidFile_ReturnsSuccess()
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
        public async Task UploadAvatar_InvalidContentType_ReturnsBadRequest()
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
        public async Task UploadAvatar_TooLarge_ReturnsBadRequest()
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
        public async Task UploadAvatar_SaveFails_ReturnsInternalServerError()
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

    }
}