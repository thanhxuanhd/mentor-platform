using Application.Services.Availabilities;
using Contract.Dtos.Availabilities.Responses;
using Contract.Repositories;
using Domain.Entities;
using Moq;
using System.Net;

namespace Application.Test;

[TestFixture]
public class AvailabilityServiceTests
{
    private Mock<IAvailabilityRepository> _mockAvailabilityRepository = null!;
    private AvailabilityService _availabilityService = null!;

    [SetUp]
    public void Setup()
    {
        _mockAvailabilityRepository = new Mock<IAvailabilityRepository>();
        _availabilityService = new AvailabilityService(_mockAvailabilityRepository.Object);
    }

    [Test]
    public async Task GetAllAvailabilitiesAsync_ReturnsListOfAvailabilities()
    {
        // Arrange
        var availabilities = new List<Availability>
        {
            new Availability { Id = Guid.NewGuid(), Name = "Morning" },
            new Availability { Id = Guid.NewGuid(), Name = "Evening" }
        };
        var expectedResponses = availabilities.Select(a => new GetAvailabilityResponse(a.Id, a.Name)).ToList();
        var queryable = availabilities.AsQueryable();

        _mockAvailabilityRepository.Setup(r => r.GetAll()).Returns(queryable);
        _mockAvailabilityRepository.Setup(r => r.ToListAsync(It.IsAny<IQueryable<GetAvailabilityResponse>>()))
            .ReturnsAsync((IQueryable<GetAvailabilityResponse> q) => q.ToList());

        // Act
        var result = await _availabilityService.GetAllAvailabilitiesAsync();

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
