using Contract.Services;
using Application.Services.Users;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
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
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _userService = new UserService(_mockUserRepository.Object, _emailServiceMock.Object);
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
    }
}