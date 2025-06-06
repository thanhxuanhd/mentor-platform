using Application.Services.TeachingApproaches;
using Contract.Dtos.TeachingApproaches.Responses;
using Contract.Shared;
using MentorPlatformAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Test
{
    [TestFixture]
    public class TeachingApproachesControllerTests
    {
        private Mock<ITeachingApproachService> _teachingApproachServiceMock;
        private TeachingApproachesController _controller;

        [SetUp]
        public void Setup()
        {
            _teachingApproachServiceMock = new Mock<ITeachingApproachService>();
            _controller = new TeachingApproachesController(_teachingApproachServiceMock.Object);
        }

        [Test]
        public async Task GetAllTeachingApproachsAsync_WhenServiceReturnsData_ReturnsOkResultWithData()
        {
            // Arrange
            var teachingApproaches = new List<GetTeachingApproachResponse>
            {
                new GetTeachingApproachResponse(Id: Guid.NewGuid(), Name: "Approach 1"),
                new GetTeachingApproachResponse(Id: Guid.NewGuid(), Name: "Approach 2")
            };
            var serviceResult = Result.Success(teachingApproaches, HttpStatusCode.OK);

            _teachingApproachServiceMock.Setup(s => s.GetAllTeachingApproachesAsync())
                .ReturnsAsync(serviceResult);

            // Act
            var actionResult = await _controller.GetAllTeachingApproachsAsync();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var objectResult = actionResult as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(objectResult.Value, Is.EqualTo(serviceResult));
            _teachingApproachServiceMock.Verify(s => s.GetAllTeachingApproachesAsync(), Times.Once);
        }
    }
}