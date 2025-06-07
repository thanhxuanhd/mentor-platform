using Application.Services.Expertises;
using Contract.Dtos.Expertises.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace WebAPI.Test
{
    [TestFixture]
    public class ExpertisesControllerTests
    {
        private Mock<IExpertiseService> _expertiseServiceMock;
        private ExpertisesController _controller;

        [SetUp]
        public void Setup()
        {
            _expertiseServiceMock = new Mock<IExpertiseService>();
            _controller = new ExpertisesController(_expertiseServiceMock.Object);
        }

        [Test]
        public async Task GetAllExpertisesAsync_WhenServiceReturnsData_ReturnsOkResultWithData()
        {
            // Arrange
            var expertises = new List<GetExpertiseResponse>
            {
                new GetExpertiseResponse(Id: Guid.NewGuid(), Name: "Expertise 1"),
                new GetExpertiseResponse(Id: Guid.NewGuid(), Name: "Expertise 2")
            };
            var serviceResult = Result.Success(expertises, HttpStatusCode.OK);

            _expertiseServiceMock.Setup(s => s.GetAllExpertisesAsync()).ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.GetAllExpertisesAsync();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _expertiseServiceMock.Verify(s => s.GetAllExpertisesAsync(), Times.Once);
        }
    }
}