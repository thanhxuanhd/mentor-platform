using System.Net;
using Application.Services.CourseItems;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Test;

[TestFixture]
public class CourseItemServiceTests
{
    [SetUp]
    public void Setup()
    {
        _courseItemRepositoryMock = new Mock<ICourseItemRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _courseItemService = new CourseItemService(_courseItemRepositoryMock.Object, _courseRepositoryMock.Object);
    }

    private Mock<ICourseItemRepository> _courseItemRepositoryMock = null!;
    private Mock<ICourseRepository> _courseRepositoryMock = null!;
    private CourseItemService _courseItemService = null!;

    [Test]
    public async Task GetAllByCourseIdAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act        
        var result = await _courseItemService.GetAllByCourseIdAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo($"Course with id = {courseId} not found"));
        }
    }

    [Test]
    public async Task GetAllByCourseIdAsync_CourseExists_ReturnsAllItems()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var items = new List<CourseItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Item 1",
                Description = "Description 1",
                MediaType = CourseMediaType.Video,
                WebAddress = "https://example.com/1",
                CourseId = courseId
            }
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        var query = items.AsQueryable();
        _courseItemRepositoryMock.Setup(repo => repo.GetAll())
            .Returns(query);

        CourseItem capturedItem = null!;
        _courseItemRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CourseItem>()))
            .Callback<CourseItem>(c => capturedItem = c)
            .Returns(Task.CompletedTask);

        _courseItemRepositoryMock.Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseItemResponse>>()))
            .ReturnsAsync(items.Select(i => i.ToCourseItemResponse()).ToList());

        // Act
        var result = await _courseItemService.GetAllByCourseIdAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Has.Count.EqualTo(1));
            Assert.That(result.Value[0].Title, Is.EqualTo("Item 1"));
        }

        _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
        _courseItemRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        _courseItemRepositoryMock.Verify(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseItemResponse>>()),
            Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
            .ReturnsAsync(default(CourseItem)); // Act
        var result = await _courseItemService.GetByIdAsync(itemId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo($"Course item with id = {itemId} not found"));
        }
    }

    [Test]
    public async Task GetByIdAsync_ItemExists_ReturnsResource()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new CourseItem
        {
            Id = itemId,
            Title = "Test Item",
            Description = "Test Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "https://example.com"
        };

        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
            .ReturnsAsync(item); // Act
        var result = await _courseItemService.GetByIdAsync(itemId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value!.Id, Is.EqualTo(itemId));
            Assert.That(result.Value.Title, Is.EqualTo("Test Item"));
        }
    }

    [Test]
    public async Task CreateAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CourseItemCreateRequest
        {
            Title = "New Item",
            Description = "Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "https://example.com"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course)); // Act
        var result = await _courseItemService.CreateAsync(courseId, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo($"Course with id = {courseId} not found"));
        }
    }

    [Test]
    public async Task CreateAsync_ValidRequest_CreatesAndReturnsResource()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var request = new CourseItemCreateRequest
        {
            Title = "New Item",
            Description = "Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "https://example.com"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        CourseItem? capturedItem = null;
        _courseItemRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CourseItem>()))
            .Callback<CourseItem>(item => capturedItem = item);

        // Act
        var result = await _courseItemService.CreateAsync(courseId, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Value!.Title, Is.EqualTo(request.Title));
            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
            Assert.That(result.Value.MediaType, Is.EqualTo(request.MediaType));
            Assert.That(result.Value.WebAddress, Is.EqualTo(request.WebAddress));
        }

        _courseItemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<CourseItem>()), Times.Once);
        _courseItemRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);

        Assert.That(capturedItem, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(capturedItem!.CourseId, Is.EqualTo(courseId));
            Assert.That(capturedItem.Title, Is.EqualTo(request.Title));
            Assert.That(capturedItem.Description, Is.EqualTo(request.Description));
            Assert.That(capturedItem.MediaType, Is.EqualTo(request.MediaType));
            Assert.That(capturedItem.WebAddress, Is.EqualTo(request.WebAddress));
        }
    }

    [Test]
    public async Task UpdateAsync_ItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "https://example.com/updated"
        };

        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
            .ReturnsAsync(default(CourseItem)); // Act
        var result = await _courseItemService.UpdateAsync(itemId, request);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo($"Course item with id = {itemId} not found"));
        }
    }

    [Test]
    public async Task UpdateAsync_ValidRequest_UpdatesAndReturnsResource()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new CourseItem
        {
            Id = itemId,
            Title = "Old Title",
            Description = "Old Description",
            MediaType = CourseMediaType.Pdf,
            WebAddress = "https://example.com/old",
            CourseId = Guid.NewGuid()
        };
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "https://example.com/updated"
        };

        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _courseItemService.UpdateAsync(itemId, request); // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value!.Title, Is.EqualTo(request.Title));
            Assert.That(existingItem.Title, Is.EqualTo(request.Title));
            Assert.That(existingItem.Description, Is.EqualTo(request.Description));
            Assert.That(existingItem.MediaType, Is.EqualTo(request.MediaType));
            Assert.That(existingItem.WebAddress, Is.EqualTo(request.WebAddress));
        }

        _courseItemRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
            .ReturnsAsync(default(CourseItem));

        // Act
        var result = await _courseItemService.DeleteAsync(itemId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo($"Course item with id = {itemId} not found"));
        }
    }

    [Test]
    public async Task DeleteAsync_ItemExists_DeletesAndReturnsSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new CourseItem
        {
            Id = itemId,
            Title = "Item to Delete",
            Description = "Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "https://example.com",
            CourseId = Guid.NewGuid()
        };

        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _courseItemService.DeleteAsync(itemId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _courseItemRepositoryMock.Verify(repo => repo.Delete(existingItem), Times.Once);
            _courseItemRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }

    [Test]
    public async Task GetAllByCourseIdAsync_ReturnsItemsInOrder()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var items = new List<CourseItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Item 2",
                Description = "Description 2",
                MediaType = CourseMediaType.Video,
                WebAddress = "https://example.com/2",
                CourseId = courseId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Item 1",
                Description = "Description 1",
                MediaType = CourseMediaType.Pdf,
                WebAddress = "https://example.com/1",
                CourseId = courseId
            }
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        var query = items.AsQueryable();
        _courseItemRepositoryMock.Setup(repo => repo.GetAll())
            .Returns(query);

        _courseItemRepositoryMock.Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseItemResponse>>()))
            .ReturnsAsync(items.OrderBy(i => i.Id).Select(i => i.ToCourseItemResponse()).ToList());

        // Act
        var result = await _courseItemService.GetAllByCourseIdAsync(courseId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Has.Count.EqualTo(2));
            // Verify items are ordered by Id
            Assert.That(result.Value, Is.Ordered.By("Id"));
        }
    }

}