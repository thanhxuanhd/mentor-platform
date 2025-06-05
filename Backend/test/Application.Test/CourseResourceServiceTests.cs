//using Application.Services.CourseResources;
//using Contract.Dtos.CourseResources.Requests;
//using Contract.Dtos.CourseResources.Responses;
//using Contract.Dtos.Courses.Responses;
//using Contract.Repositories;
//using Domain.Entities;
//using Domain.Enums;
//using Microsoft.AspNetCore.Hosting;
//using Moq;
//using System.Net;

//namespace Application.Test;

//[TestFixture]
//public class CourseResourceServiceTests
//{
//    [SetUp]
//    public void Setup()
//    {

//        _courseResourceRepositoryMock = new Mock<ICourseResourceRepository>();
//        _courseRepositoryMock = new Mock<ICourseRepository>();
//        _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
//        _courseResourceService = new CourseResourceService(
//            _courseResourceRepositoryMock.Object,
//            _courseRepositoryMock.Object,
//            _webHostEnvironmentMock.Object
//        );
//    }

//    private Mock<ICourseResourceRepository> _courseResourceRepositoryMock = null!;
//    private Mock<ICourseRepository> _courseRepositoryMock = null!;
//    private Mock<IWebHostEnvironment> _webHostEnvironmentMock = null!;
//    private CourseResourceService _courseResourceService = null!;
//    [Test]
//    public async Task GetAllByCourseIdAsync_CourseNotFound_ReturnsNotFound()
//    {
//        // Arrange
//        var courseId = Guid.NewGuid();
//        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
//            .ReturnsAsync(default(Course));

//        // Act        
//        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
//            Assert.That(result.Error, Is.EqualTo($"Course with id = {courseId} not found"));
//        }
//    }

//    [Test]
//    public async Task GetAllByCourseIdAsync_CourseExists_ReturnsAllItems()
//    {
//        // Arrange
//        var courseId = Guid.NewGuid();
//        var course = new Course { Id = courseId };
//        var items = new List<CourseResource>
//        {
//            new()
//            {
//                Id = Guid.NewGuid(),
//                Title = "Item 1",
//                Description = "Description 1",
//                ResourceType = FileType.Video,
//                ResourceUrl = "https://example.com/1",
//                CourseId = courseId
//            }
//        };

//        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
//            .ReturnsAsync(course);

//        var query = items.AsQueryable();
//        _courseResourceRepositoryMock.Setup(repo => repo.GetAll())
//            .Returns(query);

//        CourseResource capturedItem = null!;
//        _courseResourceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CourseResource>()))
//            .Callback<CourseResource>(c => capturedItem = c)
//            .Returns(Task.CompletedTask);

//        _courseResourceRepositoryMock.Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseResourceResponse>>()))
//            .ReturnsAsync(items.Select(i => i.ToCourseResourceResponse()).ToList());

//        // Act
//        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(result.Value, Has.Count.EqualTo(1));
//            Assert.That(result.Value[0].Title, Is.EqualTo("Item 1"));
//        }

//        _courseRepositoryMock.Verify(repo => repo.GetByIdAsync(courseId, null), Times.Once);
//        _courseResourceRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
//        _courseResourceRepositoryMock.Verify(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseResourceResponse>>()),
//            Times.Once);
//    }

//    [Test]
//    public async Task GetByIdAsync_ItemNotFound_ReturnsNotFound()
//    {
//        // Arrange
//        var itemId = Guid.NewGuid();
//        _courseResourceRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
//            .ReturnsAsync(default(CourseResource)); // Act
//        var result = await _courseResourceService.GetByIdAsync(itemId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
//            Assert.That(result.Error, Is.EqualTo($"Course item with id = {itemId} not found"));
//        }
//    }

//    [Test]
//    public async Task GetByIdAsync_ItemExists_ReturnsResource()
//    {
//        // Arrange
//        var itemId = Guid.NewGuid();
//        var item = new CourseResource
//        {
//            Id = itemId,
//            Title = "Test Item",
//            Description = "Test Description",
//            ResourceType = FileType.Video,
//            ResourceUrl = "https://example.com"
//        };

//        _courseResourceRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
//            .ReturnsAsync(item); // Act
//        var result = await _courseResourceService.GetByIdAsync(itemId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(result.Value!.Id, Is.EqualTo(itemId));
//            Assert.That(result.Value.Title, Is.EqualTo("Test Item"));
//        }
//    }

//    [Test]
//    public async Task CreateAsync_CourseNotFound_ReturnsNotFound()
//    {
//        // Arrange
//        var courseId = Guid.NewGuid();
//        var request = new CourseResourceRequest
//        {
//            Title = "New Item",
//            Description = "Description",
//            ResourceType = FileType.Video,
//            ResourceUrl = "https://example.com"
//        };

//        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
//            .ReturnsAsync(default(Course)); // Act
//        var result = await _courseResourceService.CreateAsync(courseId, request);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
//            Assert.That(result.Error, Is.EqualTo($"Course with id = {courseId} not found"));
//        }
//    }

//    [Test]
//    public async Task CreateAsync_ValidRequest_CreatesAndReturnsResource()
//    {
//        // Arrange
//        var courseId = Guid.NewGuid();
//        var course = new Course { Id = courseId };
//        var request = new CourseResourceRequest
//        {
//            Title = "New Item",
//            Description = "Description",
//            ResourceType = FileType.Video,
//            ResourceUrl = "https://example.com"
//        };

//        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
//            .ReturnsAsync(course);

//        CourseResource? capturedItem = null;
//        _courseResourceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CourseResource>()))
//            .Callback<CourseResource>(item => capturedItem = item);

//        // Act
//        var result = await _courseResourceService.CreateAsync(courseId, request);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
//            Assert.That(result.Value!.Title, Is.EqualTo(request.Title));
//            Assert.That(result.Value.Description, Is.EqualTo(request.Description));
//            Assert.That(result.Value.ResourceType, Is.EqualTo(request.ResourceType));
//            Assert.That(result.Value.ResourceUrl, Is.EqualTo(request.ResourceUrl));
//        }

//        _courseResourceRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<CourseResource>()), Times.Once);
//        _courseResourceRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);

//        Assert.That(capturedItem, Is.Not.Null);
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(capturedItem!.CourseId, Is.EqualTo(courseId));
//            Assert.That(capturedItem.Title, Is.EqualTo(request.Title));
//            Assert.That(capturedItem.Description, Is.EqualTo(request.Description));
//            Assert.That(capturedItem.ResourceType, Is.EqualTo(request.ResourceType));
//            Assert.That(capturedItem.ResourceUrl, Is.EqualTo(request.ResourceUrl));
//        }
//    }

//    [Test]
//    public async Task UpdateAsync_ItemNotFound_ReturnsNotFound()
//    {
//        // Arrange
//        var itemId = Guid.NewGuid();
//        var request = new CourseResourceUpdateRequest
//        {
//            Title = "Updated Item",
//            Description = "Updated Description",
//            ResourceType = FileType.Video,
//            ResourceUrl = "https://example.com/updated"
//        };

//        _courseResourceRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
//            .ReturnsAsync(default(CourseResource)); // Act
//        var result = await _courseResourceService.UpdateAsync(itemId, request);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
//            Assert.That(result.Error, Is.EqualTo($"Course item with id = {itemId} not found"));
//        }
//    }

//    [Test]
//    public async Task UpdateAsync_ValidRequest_UpdatesAndReturnsResource()
//    {
//        // Arrange
//        var itemId = Guid.NewGuid();
//        var existingItem = new CourseResource
//        {
//            Id = itemId,
//            Title = "Old Title",
//            Description = "Old Description",
//            ResourceType = FileType.Pdf,
//            ResourceUrl = "https://example.com/old",
//            CourseId = Guid.NewGuid()
//        };
//        var request = new CourseResourceUpdateRequest
//        {
//            Title = "Updated Item",
//            Description = "Updated Description",
//            ResourceType = FileType.Video,
//            ResourceUrl = "https://example.com/updated"
//        };

//        _courseResourceRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
//            .ReturnsAsync(existingItem);

//        // Act
//        var result = await _courseResourceService.UpdateAsync(itemId, request); // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(result.Value!.Title, Is.EqualTo(request.Title));
//            Assert.That(existingItem.Title, Is.EqualTo(request.Title));
//            Assert.That(existingItem.Description, Is.EqualTo(request.Description));
//            Assert.That(existingItem.ResourceType, Is.EqualTo(request.ResourceType));
//            Assert.That(existingItem.ResourceUrl, Is.EqualTo(request.ResourceUrl));
//        }

//        _courseResourceRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
//    }

//    [Test]
//    public async Task DeleteAsync_ItemNotFound_ReturnsNotFound()
//    {
//        // Arrange
//        var itemId = Guid.NewGuid();

//        _courseResourceRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
//            .ReturnsAsync(default(CourseResource));

//        // Act
//        var result = await _courseResourceService.DeleteAsync(itemId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.False);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
//            Assert.That(result.Error, Is.EqualTo($"Course item with id = {itemId} not found"));
//        }
//    }

//    [Test]
//    public async Task DeleteAsync_ItemExists_DeletesAndReturnsSuccess()
//    {
//        // Arrange
//        var itemId = Guid.NewGuid();
//        var existingItem = new CourseResource
//        {
//            Id = itemId,
//            Title = "Item to Delete",
//            Description = "Description",
//            ResourceType = FileType.Video,
//            ResourceUrl = "https://example.com",
//            CourseId = Guid.NewGuid()
//        };

//        _courseResourceRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId, null))
//            .ReturnsAsync(existingItem);

//        // Act
//        var result = await _courseResourceService.DeleteAsync(itemId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            _courseResourceRepositoryMock.Verify(repo => repo.Delete(existingItem), Times.Once);
//            _courseResourceRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
//        }
//    }

//    [Test]
//    public async Task GetAllByCourseIdAsync_ReturnsItemsInOrder()
//    {
//        // Arrange
//        var courseId = Guid.NewGuid();
//        var course = new Course { Id = courseId };
//        var items = new List<CourseResource>
//        {
//            new()
//            {
//                Id = Guid.NewGuid(),
//                Title = "Item 2",
//                Description = "Description 2",
//                ResourceType = FileType.Video,
//                ResourceUrl = "https://example.com/2",
//                CourseId = courseId
//            },
//            new()
//            {
//                Id = Guid.NewGuid(),
//                Title = "Item 1",
//                Description = "Description 1",
//                ResourceType = FileType.Pdf,
//                ResourceUrl = "https://example.com/1",
//                CourseId = courseId
//            }
//        };

//        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId, null))
//            .ReturnsAsync(course);

//        var query = items.AsQueryable();
//        _courseResourceRepositoryMock.Setup(repo => repo.GetAll())
//            .Returns(query);

//        _courseResourceRepositoryMock.Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseResourceResponse>>()))
//            .ReturnsAsync(items.OrderBy(i => i.Id).Select(i => i.ToCourseResourceResponse()).ToList());

//        // Act
//        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

//        // Assert
//        using (Assert.EnterMultipleScope())
//        {
//            Assert.That(result.IsSuccess, Is.True);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//            Assert.That(result.Value, Has.Count.EqualTo(2));
//            // Verify items are ordered by Id
//            Assert.That(result.Value, Is.Ordered.By("Id"));
//        }
//    }
//}