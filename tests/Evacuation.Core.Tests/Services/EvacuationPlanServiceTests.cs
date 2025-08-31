using AutoMapper;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Services;
using Evacuation.Domain.Entities;
using Moq;

namespace Evacuation.Core.Tests.Services
{
    public class EvacuationPlanServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly EvacuationPlanService _planService;

        public EvacuationPlanServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _mapperMock = new Mock<IMapper>(MockBehavior.Loose);
            _cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            _planService = new EvacuationPlanService(_unitOfWorkMock.Object, _mapperMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public void EvacuationPlanServiceConstructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
        {
            // Arrange
            IUnitOfWork? unitOfWork = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationPlanService(unitOfWork!, _mapperMock.Object, _cacheServiceMock.Object));
        }

        [Fact]
        public void EvacuationPlanServiceConstructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            // Arrange
            IMapper? mapper = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationPlanService(_unitOfWorkMock.Object, mapper!, _cacheServiceMock.Object));
        }

        [Fact]
        public void EvacuationPlanServiceConstructor_ShouldThrowArgumentNullException_WhenCacheIsNull()
        {
            // Arrange
            ICacheService? cacheService = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationPlanService(_unitOfWorkMock.Object, _mapperMock.Object, cacheService!));
        }

        [Fact]
        public async Task GeneratePlansAsync_ReturnsPlans_WhenBestVehicleExist()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 13.7563, 
                    Longitude = 100.5018, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.High,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 13.765,
                    Longitude = 100.5381,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);                     

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes,
                    NumberOfPeople = p.NumberOfPeople
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var plan = result.First();
            Assert.Equal(1, plan.ZoneId);
            Assert.Equal(1, plan.VehicleId);
            Assert.Equal(4, plan.EstimatedArrivalMinutes);
            Assert.Equal(40, plan.NumberOfPeople);
        }

        [Fact]
        public async Task GeneratePlansAsync_ThrowsArgumentException_WhenNoZonesFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync())
                .ReturnsAsync(new List<EvacuationZoneEntity>());

            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync())
                .ReturnsAsync(new List<VehicleEntity> { new VehicleEntity { VehicleId = 1 } });

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync())
                .ReturnsAsync(new List<EvacuationPlanEntity>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _planService.GeneratePlansAsync());
            Assert.Equal("No zones found.", ex.Message);
        }

        [Fact]
        public async Task GeneratePlansAsync_ThrowsArgumentException_WhenNoVehiclesFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync())
                .ReturnsAsync(new List<EvacuationZoneEntity> { new EvacuationZoneEntity { ZoneId = 1 } });

            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync())
                .ReturnsAsync(new List<VehicleEntity>());

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync())
                .ReturnsAsync(new List<EvacuationPlanEntity>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _planService.GeneratePlansAsync());
            Assert.Equal("No vehicles found.", ex.Message);
        }

        [Fact]
        public async Task GeneratePlansAsync_SkipsVehicle_WhenEtaGreaterThan60()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 100, 
                    UrgencyLevel = ZoneUrgencyLevel.VeryHigh,
                    RemainingPeople = 100,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 1,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _planService.GeneratePlansAsync());
            Assert.Equal("No suitable vehicle found for any zone.", ex.Message);
        }

        [Fact]
        public async Task GeneratePlansAsync_UsesVehicle_WhenCapacityLessThanRemainingPeople()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 13.7563, 
                    Longitude = 100.5018, 
                    NumberOfPeople = 100, 
                    UrgencyLevel = ZoneUrgencyLevel.High,
                    RemainingPeople = 100,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 13.765,
                    Longitude = 100.5381,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    NumberOfPeople = p.NumberOfPeople,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var plan = result.First();
            Assert.Equal(40, plan.NumberOfPeople);
            Assert.True(vehiclesMock.First().Capacity == plan.NumberOfPeople);
            Assert.True(zonesMock.First().RemainingPeople >= plan.NumberOfPeople);
        }

        [Fact]
        public async Task GeneratePlansAsync_SkipsVehicle_WhenCapacityDiffTooLarge()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 13.7563, 
                    Longitude = 100.5018, 
                    NumberOfPeople = 20, 
                    UrgencyLevel = ZoneUrgencyLevel.High,
                    RemainingPeople = 20,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 50,
                    Type = "bus",
                    Latitude = 13.765,
                    Longitude = 100.5381,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _planService.GeneratePlansAsync());
            Assert.Equal("No suitable vehicle found for any zone.", ex.Message);
        }

        [Fact]
        public async Task GeneratePlansAsync_ThrowsArgumentException_WhenNoBestVehicleForAnyZone()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 20, 
                    UrgencyLevel = ZoneUrgencyLevel.VeryHigh,
                    RemainingPeople = 20,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 20,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 1,
                    Speed = 60,
                    IsAvailable = true
                },
                new VehicleEntity
                {
                    VehicleId = 2,
                    Capacity = 50,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _planService.GeneratePlansAsync());
            Assert.Equal("No suitable vehicle found for any zone.", ex.Message);
        }

        [Fact]
        public async Task GeneratePlansAsync_PicksNearestVehicle_WhenMultipleAvailable()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.VeryHigh,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.2,
                    Speed = 60,
                    IsAvailable = true
                },
                new VehicleEntity
                {
                    VehicleId = 2,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    NumberOfPeople = p.NumberOfPeople,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var plan = result.First();
            Assert.Equal(2, plan.VehicleId);
        }

        [Fact]
        public async Task GeneratePlansAsync_PicksFastestVehicle_WhenDistanceEqual()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.VeryHigh,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 80,
                    IsAvailable = true
                },
                new VehicleEntity
                {
                    VehicleId = 2,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    NumberOfPeople = p.NumberOfPeople,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var plan = result.First();
            Assert.Equal(1, plan.VehicleId);
        }

        [Fact]
        public async Task GeneratePlansAsync_PicksVehicleWithClosestCapacityDiff_WhenDistanceAndEtaEqual()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.VeryHigh,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 45,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true
                },
                new VehicleEntity
                {
                    VehicleId = 2,
                    Capacity = 50,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    NumberOfPeople = p.NumberOfPeople,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var plan = result.First();
            Assert.Equal(1, plan.VehicleId);
        }

        [Fact]
        public async Task GeneratePlansAsync_ReducesRemainingPeople_WhenActivePlansExist()
        {
            // Arrange
            int lastRemaining = 80;
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 100, 
                    UrgencyLevel = ZoneUrgencyLevel.VeryHigh,
                    RemainingPeople = lastRemaining,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 2,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true
                }
            };

            var plansMock = new List<EvacuationPlanEntity>
            {
                new EvacuationPlanEntity
                {
                    PlanId = 1,
                    ZoneId = 1,
                    VehicleId = 1,
                    EstimatedArrivalMinutes = 10,
                    NumberOfPeople = 20,
                    Active = true
                }
            };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes,
                    NumberOfPeople = p.NumberOfPeople                    
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.Equal(60, lastRemaining - plansMock.First().NumberOfPeople);
            Assert.NotEmpty(result);
            Assert.Contains(result, r => r.ZoneId == zonesMock.First().ZoneId);
            var evacuatedCount = result.First().NumberOfPeople;
            Assert.True(evacuatedCount <= 60);
        }

        [Fact]
        public async Task GeneratePlansAsync_Commits_WhenPlansCreated()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.High,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes,
                    NumberOfPeople = p.NumberOfPeople
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotEmpty(result);
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once());
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once());
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never());
        }

        [Fact]
        public async Task GeneratePlansAsync_RollsBack_WhenExceptionThrown()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.High,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ThrowsAsync(new Exception("Server error."));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _planService.GeneratePlansAsync());
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once());
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once());
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never());
        }

        [Fact]
        public async Task GeneratePlansAsync_SetsCache_WhenZoneStatusNotExist()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity 
                { 
                    ZoneId = 1, 
                    Latitude = 0, 
                    Longitude = 0, 
                    NumberOfPeople = 40, 
                    UrgencyLevel = ZoneUrgencyLevel.High,
                    RemainingPeople = 40,
                }
            };

            var vehiclesMock = new List<VehicleEntity>
            {
                new VehicleEntity
                {
                    VehicleId = 1,
                    Capacity = 40,
                    Type = "bus",
                    Latitude = 0,
                    Longitude = 0.1,
                    Speed = 60,
                    IsAvailable = true,
                }
            };

            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity z) => z);

            _unitOfWorkMock.Setup(u => u.Vehicles.Update(It.IsAny<VehicleEntity>())).Returns((VehicleEntity v) => v);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.AddAsync(It.IsAny<EvacuationPlanEntity>()))
                .ReturnsAsync((EvacuationPlanEntity p) => p);

            _mapperMock.Setup(m => m.Map<EvacuationPlanResponse>(It.IsAny<EvacuationPlanEntity>()))
                .Returns((EvacuationPlanEntity p) => new EvacuationPlanResponse
                {
                    PlanId = p.PlanId,
                    ZoneId = p.ZoneId,
                    VehicleId = p.VehicleId,
                    EstimatedArrivalMinutes = p.EstimatedArrivalMinutes,
                    NumberOfPeople = p.NumberOfPeople
                });

            _cacheServiceMock.Setup(c => c.GetAsync<EvacuationStatusResponse>(It.IsAny<string>()))
                .ReturnsAsync((string s) => (EvacuationStatusResponse?)null);

            _cacheServiceMock.Setup(c => c.SetAsync<EvacuationStatusResponse>(
                It.IsAny<string>(), 
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()))
            .Returns(() => Task.CompletedTask);

            // Act
            var result = await _planService.GeneratePlansAsync();

            // Assert
            Assert.NotEmpty(result);
            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<EvacuationStatusResponse>(),
                It.IsAny<TimeSpan?>()),
                Times.Once
            );
        }

        [Fact]
        public async Task ClearPlansAsync_RemovesAllEntities_WhenCalled()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity> { new EvacuationZoneEntity() };
            var vehiclesMock = new List<VehicleEntity> { new VehicleEntity() };
            var plansMock = new List<EvacuationPlanEntity> { new EvacuationPlanEntity() };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _unitOfWorkMock.Setup(u => u.EvacuationZones.RemoveAll(zonesMock))
                .Returns((List<EvacuationZoneEntity> entities) => entities);

            _unitOfWorkMock.Setup(u => u.Vehicles.RemoveAll(vehiclesMock))
                .Returns((List<VehicleEntity> entities) => entities);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.RemoveAll(plansMock))
                .Returns((List<EvacuationPlanEntity> entities) => entities);

            _cacheServiceMock.Setup(c => c.ClearAllAsync()).Returns(Task.CompletedTask);

            // Act
            await _planService.ClearPlansAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.EvacuationZones.RemoveAll(zonesMock), Times.Once);
            _unitOfWorkMock.Verify(u => u.Vehicles.RemoveAll(vehiclesMock), Times.Once);
            _unitOfWorkMock.Verify(u => u.EvacuationPlans.RemoveAll(plansMock), Times.Once);
        }

        [Fact]
        public async Task ClearPlansAsync_ClearsCache_WhenCalled()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>();
            var vehiclesMock = new List<VehicleEntity>();
            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _unitOfWorkMock.Setup(u => u.EvacuationZones.RemoveAll(zonesMock))
                .Returns((List<EvacuationZoneEntity> entities) => entities);

            _unitOfWorkMock.Setup(u => u.Vehicles.RemoveAll(vehiclesMock))
                .Returns((List<VehicleEntity> entities) => entities);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.RemoveAll(plansMock))
                .Returns((List<EvacuationPlanEntity> entities) => entities);

            _cacheServiceMock.Setup(c => c.ClearAllAsync()).Returns(Task.CompletedTask);

            // Act
            await _planService.ClearPlansAsync();

            // Assert
            _cacheServiceMock.Verify(c => c.ClearAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ClearPlansAsync_Commits_WhenNoError()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>();
            var vehiclesMock = new List<VehicleEntity>();
            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _unitOfWorkMock.Setup(u => u.EvacuationZones.RemoveAll(zonesMock))
                .Returns((List<EvacuationZoneEntity> entities) => entities);

            _unitOfWorkMock.Setup(u => u.Vehicles.RemoveAll(vehiclesMock))
                .Returns((List<VehicleEntity> entities) => entities);

            _unitOfWorkMock.Setup(u => u.EvacuationPlans.RemoveAll(plansMock))
                .Returns((List<EvacuationPlanEntity> entities) => entities);

            _cacheServiceMock.Setup(c => c.ClearAllAsync()).Returns(Task.CompletedTask);

            // Act
            await _planService.ClearPlansAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task ClearPlansAsync_RollsBack_WhenExceptionThrown()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>();
            var vehiclesMock = new List<VehicleEntity>();
            var plansMock = new List<EvacuationPlanEntity>();

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _unitOfWorkMock.Setup(u => u.Vehicles.GetAllActiveAsync()).ReturnsAsync(vehiclesMock);
            _unitOfWorkMock.Setup(u => u.EvacuationPlans.GetAllActiveAsync()).ReturnsAsync(plansMock);

            _unitOfWorkMock.Setup(u => u.EvacuationZones.RemoveAll(zonesMock))
                .Throws(new Exception("Server error."));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _planService.ClearPlansAsync());
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
