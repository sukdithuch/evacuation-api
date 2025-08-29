using Evacuation.API.Controllers;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Evacuation.API.Tests.Controllers
{
    public class EvacuationsControllerTests
    {
        private readonly Mock<IEvacuationPlanService> _planServiceMock;
        private readonly Mock<IEvacuationStatusService> _statusServiceMock;
        private readonly EvacuationsController _controller;

        public EvacuationsControllerTests()
        {
            _planServiceMock = new Mock<IEvacuationPlanService>(MockBehavior.Strict);
            _statusServiceMock = new Mock<IEvacuationStatusService>(MockBehavior.Strict);
            _controller = new EvacuationsController(_planServiceMock.Object, _statusServiceMock.Object);
        }

        [Fact]
        public void EvacuationsControllerConstructor_ShouldReturnNullException_WhenServiceIsNull()
        {
            // Arrange
            IEvacuationPlanService? planService = null;
            IEvacuationStatusService? statusService = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationsController(planService!, statusService!));
        }

        [Fact]
        public async Task GeneratePlan_ReturnOk_WhenPlansGenerated()
        {
            // Arrange
            var plansMock = new List<EvacuationPlanResponse>
            {
                new EvacuationPlanResponse { PlanId = 1 }
            };

            _planServiceMock.Setup(s => s.GeneratePlansAsync()).ReturnsAsync(plansMock);

            // Act
            var result = await _controller.GeneratePlan();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPlans = Assert.IsType<List<EvacuationPlanResponse>>(okResult.Value);
            Assert.Single(returnedPlans);
            _planServiceMock.Verify(s => s.GeneratePlansAsync(), Times.Once);
        }

        [Fact]
        public async Task GeneratePlan_ReturnNotFound_WhenArgumentExceptionThrow()
        {
            // Arrange
            _planServiceMock.Setup(s => s.GeneratePlansAsync()).ThrowsAsync(new ArgumentException("No suitable vehicle found for any zone."));

            // Act
            var result = await _controller.GeneratePlan();

            // Assert
            var notFound = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
            Assert.Equal("No suitable vehicle found for any zone.", notFound.Value);
            _planServiceMock.Verify(s => s.GeneratePlansAsync(), Times.Once);
        }

        [Fact]
        public async Task GeneratePlan_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            _planServiceMock.Setup(s => s.GeneratePlansAsync()).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.GeneratePlan();

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _planServiceMock.Verify(s => s.GeneratePlansAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStatus_ReturnOk_WhenStatusesExist()
        {
            // Arrange
            var statusesMock = new List<EvacuationStatusResponse>
            {
                new EvacuationStatusResponse { ZoneId = 1 }
            };

            _statusServiceMock.Setup(s => s.GetStatusesAsync()).ReturnsAsync(statusesMock);

            // Act
            var result = await _controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStatuses = Assert.IsType<List<EvacuationStatusResponse>>(okResult.Value);
            Assert.Single(returnedStatuses);
            _statusServiceMock.Verify(s => s.GetStatusesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStatus_ReturnOk_WhenNoStatuses()
        {
            // Arrange
            _statusServiceMock.Setup(s => s.GetStatusesAsync()).ReturnsAsync(new List<EvacuationStatusResponse>());

            // Act
            var result = await _controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStatuses = Assert.IsType<List<EvacuationStatusResponse>>(okResult.Value);
            Assert.Empty(returnedStatuses);
            _statusServiceMock.Verify(s => s.GetStatusesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStatus_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            _statusServiceMock.Setup(s => s.GetStatusesAsync()).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.GetStatus();

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _statusServiceMock.Verify(s => s.GetStatusesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_ReturnOk_WhenValidRequest()
        {
            // Arrange
            var request = new EvacuationStatusRequest { ZoneId = 1, VehicleId = 2, EvacuatedPeople = 20 };
            _statusServiceMock.Setup(s => s.UpdateStatusAsync(request)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStatus(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value!);
            _statusServiceMock.Verify(s => s.UpdateStatusAsync(request), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_ReturnBadRequest_WhenInvalidRequest()
        {
            // Arrange
             var request = new EvacuationStatusRequest();
            _controller.ModelState.AddModelError("ZoneId", "The ZoneId field is required.");           

            //Act
            var result = await _controller.UpdateStatus(request);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _statusServiceMock.Verify(s => s.UpdateStatusAsync(request), Times.Never);
        }

        [Fact]
        public async Task UpdateStatus_ReturnNotFound_WhenArgumentExceptionThrow()
        {
            // Arrange
            var request = new EvacuationStatusRequest { ZoneId = 1, VehicleId = 2, EvacuatedPeople = 20 };
            _statusServiceMock.Setup(s => s.UpdateStatusAsync(request)).ThrowsAsync(new ArgumentException("Zone 1 not found."));

            // Act
            var result = await _controller.UpdateStatus(request);

            // Assert
            var notFound = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
            Assert.Equal("Zone 1 not found.", notFound.Value);
            _statusServiceMock.Verify(s => s.UpdateStatusAsync(request), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            var request = new EvacuationStatusRequest();
            _statusServiceMock.Setup(s => s.UpdateStatusAsync(request)).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.UpdateStatus(request);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _statusServiceMock.Verify(s => s.UpdateStatusAsync(request), Times.Once);
        }

        [Fact]
        public async Task ClearPlans_ReturnOk_WhenUpdateSuccessful()
        {
            // Arrange
            _planServiceMock.Setup(s => s.ClearPlansAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ClearPlans();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value!);
            _planServiceMock.Verify(s => s.ClearPlansAsync(), Times.Once);
        }

        [Fact]
        public async Task ClearPlans_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            _planServiceMock.Setup(s => s.ClearPlansAsync()).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.ClearPlans();

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _planServiceMock.Verify(s => s.ClearPlansAsync(), Times.Once);
        }
    }
}