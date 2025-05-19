using System.Net;
using Application.Services.CourseItems;
using Contract.Dtos.CourseItems.Requests;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Moq;
using NUnit.Framework;

namespace Application.Test;

[TestFixture]
public class CourseItemServiceTests
{
    private Mock<ICourseItemRepository> _courseItemRepositoryMock = null!;
    private Mock<ICourseRepository> _courseRepositoryMock = null!;
    private CourseItemService _courseItemService = null!;

    [SetUp]
    public void Setup()
    {
        _courseItemRepositoryMock = new Mock<ICourseItemRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _courseItemService = new CourseItemService(_courseItemRepositoryMock.Object, _courseRepositoryMock.Object);
    }

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
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
        });
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
                WebAddress = "http://example.com/1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Item 2",
                Description = "Description 2",
                MediaType = CourseMediaType.ExternalWebAddress,
                WebAddress = "http://example.com/2"
            }
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetAllByCourseIdAsync(courseId))
            .ReturnsAsync(items);

        // Act
        var result = await _courseItemService.GetAllByCourseIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Has.Count.EqualTo(2));
            Assert.That(result.Value[0].Title, Is.EqualTo("Item 1"));
            Assert.That(result.Value[1].Title, Is.EqualTo("Item 2"));
        });
    }

    [Test]
    public async Task GetByIdAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseItemService.GetByIdAsync(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
        });
    }

    [Test]
    public async Task GetByIdAsync_ResourceNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(default(CourseItem));

        // Act
        var result = await _courseItemService.GetByIdAsync(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Resource not found"));
        });
    }

    [Test]
    public async Task GetByIdAsync_ResourceExists_ReturnsResource()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var item = new CourseItem
        {
            Id = resourceId,
            Title = "Test Item",
            Description = "Test Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(item);

        // Act
        var result = await _courseItemService.GetByIdAsync(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Id, Is.EqualTo(resourceId));
            Assert.That(result.Value.Title, Is.EqualTo("Test Item"));
        });
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
            WebAddress = "http://example.com"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseItemService.CreateAsync(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
        });
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
            WebAddress = "http://example.com"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        CourseItem? savedItem = null;
        _courseItemRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CourseItem>()))
            .Callback<CourseItem>(item => savedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseItemService.CreateAsync(courseId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(result.Value.Title, Is.EqualTo(request.Title));
            Assert.That(savedItem, Is.Not.Null);
            Assert.That(savedItem!.CourseId, Is.EqualTo(courseId));
        });
    }

    [Test]
    public async Task UpdateAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com/updated"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseItemService.UpdateAsync(courseId, resourceId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
        });
    }

    [Test]
    public async Task UpdateAsync_ResourceNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com/updated"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(default(CourseItem));

        // Act
        var result = await _courseItemService.UpdateAsync(courseId, resourceId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Resource not found"));
        });
    }

    [Test]
    public async Task UpdateAsync_ValidRequest_UpdatesAndReturnsResource()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var existingItem = new CourseItem
        {
            Id = resourceId,
            Title = "Old Title",
            Description = "Old Description",
            MediaType = CourseMediaType.Pdf,
            WebAddress = "http://example.com/old"
        };
        var request = new CourseItemUpdateRequest
        {
            Title = "Updated Item",
            Description = "Updated Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com/updated"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _courseItemService.UpdateAsync(courseId, resourceId, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Title, Is.EqualTo(request.Title));
            Assert.That(existingItem.Title, Is.EqualTo(request.Title));
            Assert.That(existingItem.Description, Is.EqualTo(request.Description));
            Assert.That(existingItem.MediaType, Is.EqualTo(request.MediaType));
            Assert.That(existingItem.WebAddress, Is.EqualTo(request.WebAddress));
        });
    }

    [Test]
    public async Task DeleteAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(default(Course));

        // Act
        var result = await _courseItemService.DeleteAsync(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Course not found"));
        });
    }

    [Test]
    public async Task DeleteAsync_ResourceNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = courseId };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(default(CourseItem));

        // Act
        var result = await _courseItemService.DeleteAsync(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error, Is.EqualTo("Resource not found"));
        });
    }

    [Test]
    public async Task DeleteAsync_ResourceExists_DeletesAndReturnsSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = courseId };
        var existingItem = new CourseItem
        {
            Id = resourceId,
            Title = "Item to Delete",
            Description = "Description",
            MediaType = CourseMediaType.Video,
            WebAddress = "http://example.com"
        };

        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);
        _courseItemRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, resourceId))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _courseItemService.DeleteAsync(courseId, resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            _courseItemRepositoryMock.Verify(repo => repo.Delete(existingItem), Times.Once);
            _courseItemRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        });
    }
}
