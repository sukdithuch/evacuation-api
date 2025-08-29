using Evacuation.API.Controllers;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Evacuation.API.Tests.Controllers
{
    public class EvacuationZonesControllerTests
    {
        private readonly Mock<IEvacuationZoneService> _zoneServiceMock;
        private readonly EvacuationZonesController _controller;

        public EvacuationZonesControllerTests()
        {
            _zoneServiceMock = new Mock<IEvacuationZoneService>(MockBehavior.Strict);
            _controller = new EvacuationZonesController(_zoneServiceMock.Object);
        }

        [Fact]
        public void EvacuationZonesControllerConstructor_ShouldReturnNullException_WhenServiceIsNull()
        {
            // Arrange
            IEvacuationZoneService? zoneService = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationZonesController(zoneService!));
        }

        [Fact]
        public async Task GetAll_ReturnOk_WhenVehiclesExist()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneResponse>
            {
                new EvacuationZoneResponse { ZoneId = 1 }
            };

            _zoneServiceMock.Setup(s => s.GetEvacuationZonesAsync()).ReturnsAsync(zonesMock);

            //Act
            var result = await _controller.GetAll();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedZones = Assert.IsType<List<EvacuationZoneResponse>>(okResult.Value);
            Assert.Single(returnedZones);
            _zoneServiceMock.Verify(s => s.GetEvacuationZonesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnOk_WhenNoVehicles()
        {
            // Arrange
            _zoneServiceMock.Setup(s => s.GetEvacuationZonesAsync()).ReturnsAsync(new List<EvacuationZoneResponse>());

            //Act
            var result = await _controller.GetAll();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedZones = Assert.IsType<List<EvacuationZoneResponse>>(okResult.Value);
            Assert.Empty(returnedZones);
            _zoneServiceMock.Verify(s => s.GetEvacuationZonesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            _zoneServiceMock.Setup(s => s.GetEvacuationZonesAsync()).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _zoneServiceMock.Verify(s => s.GetEvacuationZonesAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnOk_WhenValidRequest()
        {
            // Arrange
            var request = new EvacuationZoneRequest { UrgencyLevel = 5, NumberOfPeople = 20};
            var response = new EvacuationZoneResponse { ZoneId = 1, UrgencyLevel = 5, NumberOfPeople = 20 }; 

            _zoneServiceMock.Setup(s => s.CreateEvacuationZoneAsync(request)).ReturnsAsync(response);

            //Act
            var result = await _controller.Create(request);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedZones = Assert.IsType<EvacuationZoneResponse>(okResult.Value);
            Assert.Equal(1, returnedZones.ZoneId);
            Assert.Equal(5, returnedZones.UrgencyLevel);
            Assert.Equal(20, returnedZones.NumberOfPeople);
            _zoneServiceMock.Verify(s => s.CreateEvacuationZoneAsync(request), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnBadRequest_WhenInvalidRequest()
        {
            // Arrange
             var request = new EvacuationZoneRequest();
            _controller.ModelState.AddModelError("UrgencyLevel", "The UrgencyLevel field is required.");           

            //Act
            var result = await _controller.Create(request);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _zoneServiceMock.Verify(s => s.CreateEvacuationZoneAsync(request), Times.Never);
        }

        [Fact]
        public async Task Create_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            var request = new EvacuationZoneRequest();
            _zoneServiceMock.Setup(s => s.CreateEvacuationZoneAsync(request)).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _zoneServiceMock.Verify(s => s.CreateEvacuationZoneAsync(request), Times.Once);
        }
    }
}
