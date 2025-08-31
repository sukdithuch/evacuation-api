using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Services;
using Evacuation.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace Evacuation.Core.Tests.Services
{
    public class EvacuationStatusServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly EvacuationStatusService _statusService;

        public EvacuationStatusServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            _statusService = new EvacuationStatusService(_unitOfWorkMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public void EvacuationStatusServiceConstructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
        {
            // Arrange
            IUnitOfWork? unitOfWork = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationStatusService(unitOfWork!, _cacheServiceMock.Object));
        }

        [Fact]
        public void EvacuationStatusServiceConstructor_ShouldThrowArgumentNullException_WhenCacheIsNull()
        {
            // Arrange
            ICacheService? cacheService = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationStatusService(_unitOfWorkMock.Object, cacheService!));
        }

        [Fact]
        public async Task GetStatusesAsync_ReturnsStatuses_WhenStatusesExist()
        {
            // Arrange
            var cacheKeys = new HashSet<string> { "Z:1:STATUS" };
            _cacheServiceMock.Setup(c => c.GetAllKeysAsync()).ReturnsAsync(cacheKeys);
            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(cacheKeys.First()))
                .ReturnsAsync(new EvacuationStatusResponse { ZoneId = 1 });

            // Act
            var result = await _statusService.GetStatusesAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().ZoneId);
            _cacheServiceMock.Verify(c => c.GetAsync<EvacuationStatusResponse>(cacheKeys.First()), Times.Once());
        }

        [Fact]
        public async Task GetStatusesAsync_SetsCache_WhenCacheEmpty()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity { ZoneId = 1 },
            };

            _cacheServiceMock.Setup(c => c.GetAllKeysAsync()).ReturnsAsync(new HashSet<string>());            
            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _statusService.GetStatusesAsync();

            // Assert
            Assert.Single(result);
            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetStatusesAsync_ReturnsEmptyList_WhenNoStatuses()
        {
            // Arrange
            _cacheServiceMock.Setup(c => c.GetAllKeysAsync()).ReturnsAsync(new HashSet<string>());            
            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(new List<EvacuationZoneEntity>());

            // Act
            var result = await _statusService.GetStatusesAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_UpdatesZoneAndVehicleAndCacheAndAddLog_WhenValidRequest()
        {
            // Arrange
            var request = new DTOs.Requests.EvacuationStatusRequest { ZoneId = 1, VehicleId = 1, EvacuatedPeople = 20 };
            var zoneMock = new EvacuationZoneEntity { ZoneId = 1, TotalEvacuated = 0, RemainingPeople = 50 };
            var vehicleMock = new VehicleEntity { VehicleId = 1, Capacity = 20, IsAvailable = false };
            var planMock = new EvacuationPlanEntity { PlanId = 1, ZoneId = 1, VehicleId = 1, NumberOfPeople = 20 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(zoneMock.ZoneId)).ReturnsAsync(zoneMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(vehicleMock.VehicleId)).ReturnsAsync(vehicleMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.FindByAsync(It.IsAny<Expression<Func<EvacuationPlanEntity, bool>>>()))
                .ReturnsAsync(new List<EvacuationPlanEntity> { planMock });

            _unitOfWorkMock.Setup(u => u.EvacuationZones.Update(It.IsAny<EvacuationZoneEntity>()))
                .Returns(It.IsAny<EvacuationZoneEntity>());

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>()))
                .Returns(It.IsAny<VehicleEntity>());

            _unitOfWorkMock.Setup(u => u.EvacuationLogs.AddAsync(It.IsAny<EvacuationLogEntity>()))
                .ReturnsAsync(It.IsAny<EvacuationLogEntity>());

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            await _statusService.UpdateStatusAsync(request);

            // Assert
            Assert.Equal(request.EvacuatedPeople, zoneMock.TotalEvacuated);
            Assert.Equal(50 - request.EvacuatedPeople, zoneMock.RemainingPeople);
            Assert.True(vehicleMock.IsAvailable);

            _unitOfWorkMock.Verify(u => u.EvacuationLogs.AddAsync(It.IsAny<EvacuationLogEntity>()), Times.Once);

            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()),
                Times.Once
            );

            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateStatusAsync_ThrowsArgumentException_WhenZoneNotFound()
        {
            // Arrange
            var request = new DTOs.Requests.EvacuationStatusRequest { ZoneId = 1, VehicleId = 1, EvacuatedPeople = 20 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((EvacuationZoneEntity)null!);

            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<VehicleEntity>());

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.FindByAsync(It.IsAny<Expression<Func<EvacuationPlanEntity, bool>>>()))
                .ReturnsAsync(new List<EvacuationPlanEntity>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _statusService.UpdateStatusAsync(request));
            Assert.Equal($"Zone {request.ZoneId} not found.", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ThrowsArgumentException_WhenVehicleNotFound()
        {
            // Arrange
            var request = new DTOs.Requests.EvacuationStatusRequest { ZoneId = 1, VehicleId = 1, EvacuatedPeople = 20 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(new EvacuationZoneEntity { ZoneId = 1 });

            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((VehicleEntity)null!);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.FindByAsync(It.IsAny<Expression<Func<EvacuationPlanEntity, bool>>>()))
                .ReturnsAsync(new List<EvacuationPlanEntity>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _statusService.UpdateStatusAsync(request));
            Assert.Equal($"Vehicle {request.VehicleId} not found.", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ThrowsArgumentException_WhenPlanNotFound()
        {
            // Arrange
            var request = new DTOs.Requests.EvacuationStatusRequest { ZoneId = 1, VehicleId = 1, EvacuatedPeople = 20 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(new EvacuationZoneEntity { ZoneId = 1 });

            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new VehicleEntity { VehicleId = 1 });

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.FindByAsync(It.IsAny<Expression<Func<EvacuationPlanEntity, bool>>>()))
                .ReturnsAsync(new List<EvacuationPlanEntity>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _statusService.UpdateStatusAsync(request));
            Assert.Equal($"Plan not found for zone {request.ZoneId} and vehicle {request.VehicleId}.", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_RollsBack_WhenExceptionThrown()
        {
            // Arrange
            var request = new DTOs.Requests.EvacuationStatusRequest { ZoneId = 1, VehicleId = 1, EvacuatedPeople = 20 };
            var zoneMock = new EvacuationZoneEntity { ZoneId = 1, TotalEvacuated = 0, RemainingPeople = 50 };
            var vehicleMock = new VehicleEntity { VehicleId = 1, Capacity = 20, IsAvailable = false };
            var planMock = new EvacuationPlanEntity { PlanId = 1, ZoneId = 1, VehicleId = 1, NumberOfPeople = 20 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(zoneMock.ZoneId)).ReturnsAsync(zoneMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.FindByIdAsync(vehicleMock.VehicleId)).ReturnsAsync(vehicleMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.FindByAsync(It.IsAny<Expression<Func<EvacuationPlanEntity, bool>>>()))
                .ReturnsAsync(new List<EvacuationPlanEntity> { planMock });

            _unitOfWorkMock.Setup(u => u.EvacuationZones.Update(It.IsAny<EvacuationZoneEntity>()))
                .Throws(new Exception("Server error."));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _statusService.UpdateStatusAsync(request));
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
