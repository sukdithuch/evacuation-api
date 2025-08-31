using Evacuation.API.Controllers;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Evacuation.API.Tests.Controllers
{
    public class VehiclesControllerTests
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;
        private readonly VehiclesController _controller;

        public VehiclesControllerTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>(MockBehavior.Strict);
            _controller = new VehiclesController(_vehicleServiceMock.Object);
        }

        [Fact]
        public void EvacuationsControllerConstructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            // Arrange
            IVehicleService? vehicleService = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new VehiclesController(vehicleService!));
        }

        [Fact]
        public async Task GetAll_ReturnOk_WhenVehiclesExist()
        {
            // Arrange
            var vehiclesMock = new List<VehicleResponse>
            {
                new VehicleResponse { VehicleId = 1 }
            };

            _vehicleServiceMock.Setup(s => s.GetVehiclesAsync()).ReturnsAsync(vehiclesMock);

            //Act
            var result = await _controller.GetAll();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicles = Assert.IsType<List<VehicleResponse>>(okResult.Value);
            Assert.Single(returnedVehicles);
            _vehicleServiceMock.Verify(s => s.GetVehiclesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnOk_WhenNoVehicles()
        {
            // Arrange
            _vehicleServiceMock.Setup(s => s.GetVehiclesAsync()).ReturnsAsync(new List<VehicleResponse>());

            //Act
            var result = await _controller.GetAll();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicles = Assert.IsType<List<VehicleResponse>>(okResult.Value);
            Assert.Empty(returnedVehicles);
            _vehicleServiceMock.Verify(s => s.GetVehiclesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            _vehicleServiceMock.Setup(s => s.GetVehiclesAsync()).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _vehicleServiceMock.Verify(s => s.GetVehiclesAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnOk_WhenValidRequest()
        {
            // Arrange
            var request = new VehicleRequest { Capacity = 20, Type = "van"};
            var response = new VehicleResponse { VehicleId = 1, Capacity = 20, Type = "van" }; 

            _vehicleServiceMock.Setup(s => s.CreateVehicleAsync(request)).ReturnsAsync(response);

            //Act
            var result = await _controller.Create(request);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicle = Assert.IsType<VehicleResponse>(okResult.Value);
            Assert.Equal(1, returnedVehicle.VehicleId);
            Assert.Equal(20, returnedVehicle.Capacity);
            Assert.Equal("van", returnedVehicle.Type);
            _vehicleServiceMock.Verify(s => s.CreateVehicleAsync(request), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnBadRequest_WhenInvalidRequest()
        {
            // Arrange
             var request = new VehicleRequest();
            _controller.ModelState.AddModelError("Capacity", "The Capacity field is required.");           

            //Act
            var result = await _controller.Create(request);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _vehicleServiceMock.Verify(s => s.CreateVehicleAsync(request), Times.Never);
        }

        [Fact]
        public async Task Create_ReturnInternalServerError_WhenExceptionThrow()
        {
            // Arrange
            var request = new VehicleRequest();
            _vehicleServiceMock.Setup(s => s.CreateVehicleAsync(request)).ThrowsAsync(new Exception("Server error."));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            Assert.Equal("Server error.", serverError.Value);
            _vehicleServiceMock.Verify(s => s.CreateVehicleAsync(request), Times.Once);
        }
    }
}
