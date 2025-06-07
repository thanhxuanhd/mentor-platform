using Application.Services.Availabilities;
using Contract.Dtos.Availabilities.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace WebAPI.Test
{
    [TestFixture]
    public class AvailabilitiesControllerTests
    {
        private Mock<IAvailabilityService> _availabilityServiceMock;
        private AvailabilitiesController _controller;

        [SetUp]
        public void Setup()
        {
            _availabilityServiceMock = new Mock<IAvailabilityService>();
            _controller = new AvailabilitiesController(_availabilityServiceMock.Object);
        }

        [Test]
        public async Task GetAllAvailabilitiesAsync_WhenServiceReturnsData_ReturnsOkResultWithData()
        {
            // Arrange
            var availabilities = new List<GetAvailabilityResponse>
            {
                new GetAvailabilityResponse(Id: Guid.NewGuid(), Name: "Morning"),
                new GetAvailabilityResponse(Id: Guid.NewGuid(), Name: "Afternoon")
            };
            var serviceResult = Result.Success(availabilities, HttpStatusCode.OK);

            _availabilityServiceMock.Setup(s => s.GetAllAvailabilitiesAsync()).ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.GetAllAvailabilitiesAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
                var objectResult = actionResult as ObjectResult;
                Assert.That(objectResult, Is.Not.Null);
                Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            });
            _availabilityServiceMock.Verify(s => s.GetAllAvailabilitiesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllAvailabilitiesAsync_WhenServiceReturnsError_ReturnsStatusCodeFromService()
        {
            // Arrange
            var errorMessage = "Database error";
            var serviceResult = Result.Failure<List<GetAvailabilityResponse>>(errorMessage, HttpStatusCode.InternalServerError);

            _availabilityServiceMock.Setup(s => s.GetAllAvailabilitiesAsync()).ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.GetAllAvailabilitiesAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
                var objectResult = actionResult as ObjectResult;
                Assert.That(objectResult, Is.Not.Null);
                Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
                Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            });
            _availabilityServiceMock.Verify(s => s.GetAllAvailabilitiesAsync(), Times.Once);
        }
    }
}