using System.Net;
using Application.Services.Expertises;
using Contract.Dtos.Expertises.Responses;
using Contract.Repositories;
using Domain.Entities;
using Moq;

namespace Application.Test;

[TestFixture]
public class ExpertiseServiceTests
{
    private Mock<IExpertiseRepository> _mockExpertiseRepository = null!;
    private ExpertiseService _expertiseService = null!;

    [SetUp]
    public void Setup()
    {
        _mockExpertiseRepository = new Mock<IExpertiseRepository>();
        _expertiseService = new ExpertiseService(_mockExpertiseRepository.Object);
    }

    [Test]
    public async Task GetAllExpertisesAsync_ReturnsListOfExpertises()
    {
        // Arrange
        var expertises = new List<Expertise>
        {
            new Expertise { Id = Guid.NewGuid(), Name = "Software Development" },
            new Expertise { Id = Guid.NewGuid(), Name = "Data Science" }
        };
        var expectedResponses = expertises.Select(e => new GetExpertiseResponse(e.Id, e.Name)).ToList();
        var queryable = expertises.AsQueryable();

        _mockExpertiseRepository.Setup(r => r.GetAll()).Returns(queryable);
        _mockExpertiseRepository.Setup(r => r.ToListAsync(It.IsAny<IQueryable<GetExpertiseResponse>>()))
            .ReturnsAsync((IQueryable<GetExpertiseResponse> q) => q.ToList());

        // Act
        var result = await _expertiseService.GetAllExpertisesAsync();

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
