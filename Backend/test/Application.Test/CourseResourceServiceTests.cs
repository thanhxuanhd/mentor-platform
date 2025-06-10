using Application.Services.CourseResources;
using Contract.Dtos.CourseResources.Responses;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Net;

namespace Application.Test;

[TestFixture]
public class CourseResourceServiceTests
{
    [SetUp]
    public void Setup()
    {

        _courseResourceRepositoryMock = new Mock<ICourseResourceRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        _courseResourceService = new CourseResourceService(
            _courseResourceRepositoryMock.Object,
            _courseRepositoryMock.Object,
            _webHostEnvironmentMock.Object
        );
    }

    private Mock<ICourseResourceRepository> _courseResourceRepositoryMock = null!;
    private Mock<ICourseRepository> _courseRepositoryMock = null!;
    private Mock<IWebHostEnvironment> _webHostEnvironmentMock = null!;
    private CourseResourceService _courseResourceService = null!;

    [Test]
    public async Task GetAllByCourseIdAsync_CourseExists_ReturnsResourcesList()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            MentorId = mentorId
        };

        var resources = new List<CourseResource>
    {
        new CourseResource
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = "Resource 1",
            ResourceUrl = "http://example.com/resource1",
            ResourceType = FileType.Pdf,
            Course = course // Ensure navigation property is not null
        },
        new CourseResource
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = "Resource 2",
            ResourceUrl = "http://example.com/resource2",
            ResourceType = FileType.Video,
            Course = course // Ensure navigation property is not null
        }
    };

        _courseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        _courseResourceRepositoryMock
            .Setup(repo => repo.GetAll())
            .Returns(resources.AsQueryable());

        _courseResourceRepositoryMock
            .Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseResourceResponse>>()))
            .ReturnsAsync(resources.Select(r => r.ToCourseResourceResponse()).ToList());

        // Act
        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Count, Is.EqualTo(2));
            Assert.That(result.Value[0].Title, Is.EqualTo("Resource 1"));
            Assert.That(result.Value[1].Title, Is.EqualTo("Resource 2"));
            Assert.That(result.Value[0].CourseTitle, Is.EqualTo("Test Course"));
            Assert.That(result.Value[0].MentorId, Is.EqualTo(mentorId));
        });
    }

    [Test]
    public async Task GetAllByCourseIdAsync_CourseNotFound_ReturnsNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Error, Is.EqualTo($"Course with id = {courseId} not found"));
        });
    }

    [Test]
    public async Task GetAllByCourseIdAsync_CourseExistsButNoResources_ReturnsEmptyList()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Empty Course",
            MentorId = mentorId
        };

        var resources = new List<CourseResource>(); // Empty list

        _courseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        _courseResourceRepositoryMock
            .Setup(repo => repo.GetAll())
            .Returns(resources.AsQueryable());

        _courseResourceRepositoryMock
            .Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseResourceResponse>>()))
            .ReturnsAsync(new List<CourseResourceResponse>());

        // Act
        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value, Is.Empty);
        });
    }

    [Test]
    public async Task GetAllByCourseIdAsync_CourseExists_ReturnsResourcesWithCorrectFileTypes()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var mentorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            Title = "Mixed Resource Course",
            MentorId = mentorId
        };

        var resources = new List<CourseResource>
    {
        new CourseResource
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = "PDF Resource",
            ResourceUrl = "http://example.com/pdf",
            ResourceType = FileType.Pdf,
            Course = course
        },
        new CourseResource
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = "Video Resource",
            ResourceUrl = "http://example.com/video",
            ResourceType = FileType.Video,
            Course = course
        }
    };

        _courseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(courseId, null))
            .ReturnsAsync(course);

        _courseResourceRepositoryMock
            .Setup(repo => repo.GetAll())
            .Returns(resources.AsQueryable());

        _courseResourceRepositoryMock
            .Setup(repo => repo.ToListAsync(It.IsAny<IQueryable<CourseResourceResponse>>()))
            .ReturnsAsync(resources.Select(r => r.ToCourseResourceResponse()).ToList());

        // Act
        var result = await _courseResourceService.GetAllByCourseIdAsync(courseId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Has.Count.EqualTo(2));
            Assert.That(result.Value![0].ResourceType, Is.EqualTo(FileType.Pdf));
            Assert.That(result.Value![1].ResourceType, Is.EqualTo(FileType.Video));
        });
    }

    [Test]
    public async Task GetByIdAsync_ResourceExists_ReturnsResource()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var course = new Course { Id = Guid.NewGuid(), Title = "Test Course", MentorId = Guid.NewGuid() };
        var resource = new CourseResource
        {
            Id = resourceId,
            Title = "Resource 1",
            Description = "Description",
            ResourceType = FileType.Pdf,
            ResourceUrl = "http://example.com/resource1",
            CourseId = course.Id,
            Course = course
        };

        _courseResourceRepositoryMock
            .Setup(repo => repo.GetByIdAsync(resourceId, cr => cr.Course))
            .ReturnsAsync(resource);

        // Act
        var result = await _courseResourceService.GetByIdAsync(resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Id, Is.EqualTo(resourceId));
            Assert.That(result.Value!.Title, Is.EqualTo(resource.Title));
            Assert.That(result.Value!.CourseTitle, Is.EqualTo(course.Title));
        });
    }

    [Test]
    public async Task GetByIdAsync_ResourceNotFound_ReturnsNotFound()
    {
        // Arrange
        var resourceId = Guid.NewGuid();

        _courseResourceRepositoryMock
            .Setup(repo => repo.GetByIdAsync(resourceId, cr => cr.Course))
            .ReturnsAsync((CourseResource?)null);

        // Act
        var result = await _courseResourceService.GetByIdAsync(resourceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Error, Is.EqualTo($"Course resource with id = {resourceId} not found"));
        });
    }
}