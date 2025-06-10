using System.Net;
using Application.Services.TeachingApproaches;
using Contract.Dtos.TeachingApproaches.Responses;
using Contract.Repositories;
using Domain.Entities;
using Moq;

namespace Application.Test;

public class TeachingApproachServiceTests
{
    private Mock<ITeachingApproachRepository> _mockTeachingApproachRepository = null!;
    private TeachingApproachService _teachingApproachService = null!;

    [SetUp]
    public void Setup()
    {
        _mockTeachingApproachRepository = new Mock<ITeachingApproachRepository>();
        _teachingApproachService = new TeachingApproachService(_mockTeachingApproachRepository.Object);
    }

    [Test]
    public async Task GetAllTeachingApproachesAsync_ReturnsListOfTeachingApproaches()
    {
        // Arrange
        var teachingApproaches = new List<TeachingApproach>
        {
            new TeachingApproach { Id = Guid.NewGuid(), Name = "Project-Based Learning" },
            new TeachingApproach { Id = Guid.NewGuid(), Name = "Flipped Classroom" }
        };

        var expectedResponses = teachingApproaches.Select(t => new GetTeachingApproachResponse(t.Id, t.Name)).ToList();
        var queryable = teachingApproaches.AsQueryable();
        _mockTeachingApproachRepository.Setup(r => r.GetAll()).Returns(queryable);
        _mockTeachingApproachRepository.Setup(r => r.ToListAsync(It.IsAny<IQueryable<GetTeachingApproachResponse>>()))
            .ReturnsAsync((IQueryable<GetTeachingApproachResponse> q) => q.ToList());

        // Act
        var result = await _teachingApproachService.GetAllTeachingApproachesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Count, Is.EqualTo(expectedResponses.Count));
            foreach (var expected in expectedResponses)
            {
                Assert.That(result.Value.Any(r => r.Id == expected.Id && r.Name == expected.Name), Is.True);
            }
        });
    }
}
