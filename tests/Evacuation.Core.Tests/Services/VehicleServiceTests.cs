using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Services;
using Evacuation.Domain.Entities;
using Moq;

namespace Evacuation.Core.Tests.Services
{
    public class VehicleServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _mapperMock = new Mock<IMapper>(MockBehavior.Loose);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(It.IsAny<int>());

            _vehicleService = new VehicleService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void VehicleServiceConstructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
        {
            // Arrange
            IUnitOfWork? unitOfWork = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new VehicleService(unitOfWork!, _mapperMock.Object));
        }

        [Fact]
        public void VehicleServiceConstructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            // Arrange
            IMapper? mapper = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new VehicleService(_unitOfWorkMock.Object, mapper!));
        }

        [Fact]
        public async Task GetVehiclesAsync_ReturnsVehicles_WhenVehiclesExist()
        {
            // Arrange
            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity { VehicleId = 1 }
            };

            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllAsync()).ReturnsAsync(vehiclesMock);
            _mapperMock.Setup(m => m.Map<List<VehicleResponse>>(It.IsAny<List<VehicleEntity>>()))
                .Returns((List<VehicleEntity> entities) => 
                    entities.Select(e => new VehicleResponse
                    {
                        VehicleId = e.VehicleId
                    }).ToList());

            // Act
            var result = await _vehicleService.GetVehiclesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _unitOfWorkMock.Verify(u => u.Vehicles.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetVehiclesAsync_ReturnsEmpty_WhenNoVehiclesExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllAsync()).ReturnsAsync(new List<VehicleEntity>());
            _mapperMock.Setup(m => m.Map<List<VehicleResponse>>(It.IsAny<List<VehicleEntity>>()))
                .Returns((List<VehicleEntity> entities) => 
                    entities.Select(e => new VehicleResponse
                    {
                        VehicleId = e.VehicleId
                    }).ToList());

            // Act
            var result = await _vehicleService.GetVehiclesAsync();

            // Assert
            Assert.Empty(result);
            _unitOfWorkMock.Verify(u => u.Vehicles.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetActiveVehiclesAsync_ReturnsActiveVehicles_WhenActiveVehiclesExist()
        {
            // Arrange
            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity { VehicleId = 1, Active = true }
            };

            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _mapperMock.Setup(m => m.Map<List<VehicleResponse>>(It.IsAny<List<VehicleEntity>>()))
                .Returns((List<VehicleEntity> entities) => 
                    entities.Select(e => new VehicleResponse
                    {
                        VehicleId = e.VehicleId
                    }).ToList());

            // Act
            var result = await _vehicleService.GetActiveVehiclesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _unitOfWorkMock.Verify(u => u.Vehicles.GetAllActiveAsync(), Times.Once());
        }

        [Fact]
        public async Task GetActiveVehiclesAsync_ReturnsEmpty_WhenNoActiveVehiclesExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(new List<VehicleEntity>());
            _mapperMock.Setup(m => m.Map<List<VehicleResponse>>(It.IsAny<List<VehicleEntity>>()))
                .Returns((List<VehicleEntity> entities) => 
                    entities.Select(e => new VehicleResponse
                    {
                        VehicleId = e.VehicleId
                    }).ToList());

            // Act
            var result = await _vehicleService.GetActiveVehiclesAsync();

            // Assert
            Assert.Empty(result);
            _unitOfWorkMock.Verify(u => u.Vehicles.GetAllActiveAsync(), Times.Once());
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ReturnsVehicle_WhenFound()
        {
            // Arrange
            var vehicleMock = new VehicleEntity { VehicleId = 1 };

            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(vehicleMock.VehicleId)).ReturnsAsync(vehicleMock);
            _mapperMock.Setup(m => m.Map<VehicleResponse>(It.IsAny<VehicleEntity>()))
                .Returns((VehicleEntity e) => new VehicleResponse
                {
                    VehicleId = e.VehicleId
                });

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(vehicleMock.VehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicleMock.VehicleId, result.VehicleId);
            _unitOfWorkMock.Verify(u => u.Vehicles.FindByIdAsync(vehicleMock.VehicleId), Times.Once);
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ThrowsArgumentException_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((VehicleEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _vehicleService.GetVehicleByIdAsync(0));
            Assert.Equal("Vehicle not found.", ex.Message);
        }

        [Fact]
        public async Task CreateVehicleAsync_ReturnsCreatedVehicle()
        {
            // Arrange
            var request = new VehicleRequest { Capacity = 5 };

            _mapperMock.Setup(m => m.Map<VehicleEntity>(It.IsAny<VehicleRequest>()))
                .Returns((VehicleRequest req) => new VehicleEntity
                 {
                     Capacity = req.Capacity
                 });

            _unitOfWorkMock.Setup(u => u.Vehicles.AddAsync(It.IsAny<VehicleEntity>())).ReturnsAsync((VehicleEntity e) => e);

            _mapperMock.Setup(m => m.Map<VehicleResponse>(It.IsAny<VehicleEntity>()))
                .Returns((VehicleEntity e) => new VehicleResponse
                {
                    VehicleId = e.VehicleId,
                    Capacity = e.Capacity
                });

            // Act
            var result = await _vehicleService.CreateVehicleAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Capacity, result.Capacity);
            _unitOfWorkMock.Verify(u => u.Vehicles.AddAsync(It.IsAny<VehicleEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateVehicleAsync_UpdatesVehicle_WhenFound()
        {
            // Arrange
            var request = new VehicleRequest { Capacity = 5 };
            var vehicleMock = new VehicleEntity { VehicleId = 1, Capacity = 7 };

            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(vehicleMock.VehicleId)).ReturnsAsync(vehicleMock);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity e) => e);

            _mapperMock.Setup(m => m.Map<VehicleResponse>(It.IsAny<VehicleEntity>()))
                .Returns((VehicleEntity e) => new VehicleResponse
                {
                    VehicleId = e.VehicleId,
                    Capacity = e.Capacity
                });

            // Act
            var result = await _vehicleService.UpdateVehicleAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Capacity, result.Capacity);
            _unitOfWorkMock.Verify(u => u.Vehicles.Update(It.IsAny<VehicleEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateVehicleAsync_ThrowsArgumentException_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((VehicleEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _vehicleService.UpdateVehicleAsync(It.IsAny<int>(), It.IsAny<VehicleRequest>()));
            Assert.Equal("Vehicle not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteVehicleAsync_RemovesVehicle_WhenFound()
        {
            // Arrange
            var vehicleMock = new VehicleEntity { VehicleId = 1, Capacity = 10 };

            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(vehicleMock.VehicleId)).ReturnsAsync(vehicleMock);

            _unitOfWorkMock.Setup(u => u.Vehicles.Remove(It.IsAny<VehicleEntity>())).Returns((VehicleEntity e) => e);

            _mapperMock.Setup(m => m.Map<VehicleResponse>(It.IsAny<VehicleEntity>()))
                .Returns((VehicleEntity e) => new VehicleResponse
                {
                    VehicleId = e.VehicleId,
                    Capacity = e.Capacity     
                });

            // Act
            var result = await _vehicleService.DeleteVehicleAsync(vehicleMock.VehicleId);

            // Assert
            Assert.NotNull(result);
            _unitOfWorkMock.Verify(u => u.Vehicles.Remove(It.IsAny<VehicleEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteVehicleAsync_ThrowsArgumentException_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((VehicleEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _vehicleService.DeleteVehicleAsync(It.IsAny<int>()));
            Assert.Equal("Vehicle not found.", ex.Message);
        }
    }
}
