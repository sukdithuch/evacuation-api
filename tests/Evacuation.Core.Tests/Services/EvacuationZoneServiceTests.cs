using AutoMapper;
using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.DTOs.Responses;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Core.Services;
using Evacuation.Domain.Entities;
using Moq;

namespace Evacuation.Core.Tests.Services
{
    public class EvacuationZoneServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EvacuationZoneService _zoneService;

        public EvacuationZoneServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _mapperMock = new Mock<IMapper>(MockBehavior.Loose);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(It.IsAny<int>());

            _zoneService = new EvacuationZoneService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void EvacuationZoneServiceConstructor_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
        {
            // Arrange
            IUnitOfWork? unitOfWork = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationZoneService(unitOfWork!, _mapperMock.Object));
        }

        [Fact]
        public void EvacuationZoneServiceConstructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
        {
            // Arrange
            IMapper? mapper = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new EvacuationZoneService(_unitOfWorkMock.Object, mapper!));
        }

        [Fact]
        public async Task GetEvacuationZonesAsync_ReturnsZones_WhenZonesExist()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity { ZoneId = 1 }
            };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllAsync()).ReturnsAsync(zonesMock);
            _mapperMock.Setup(m => m.Map<List<EvacuationZoneResponse>>(It.IsAny<List<EvacuationZoneEntity>>()))
                .Returns((List<EvacuationZoneEntity> entities) => 
                    entities.Select(e => new EvacuationZoneResponse
                    {
                        ZoneId = e.ZoneId
                    }).ToList());

            // Act
            var result = await _zoneService.GetEvacuationZonesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetEvacuationZonesAsync_ReturnsEmpty_WhenNoZonesExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllAsync()).ReturnsAsync(new List<EvacuationZoneEntity>());
            _mapperMock.Setup(m => m.Map<List<EvacuationZoneResponse>>(It.IsAny<List<EvacuationZoneEntity>>()))
                .Returns((List<EvacuationZoneEntity> entities) => 
                    entities.Select(e => new EvacuationZoneResponse
                    {
                        ZoneId = e.ZoneId
                    }).ToList());

            // Act
            var result = await _zoneService.GetEvacuationZonesAsync();

            // Assert
            Assert.Empty(result);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetActiveEvacuationZonesAsync_ReturnsActiveZones_WhenActiveZonesExist()
        {
            // Arrange
            var zonesMock = new List<EvacuationZoneEntity>
            {
                new EvacuationZoneEntity { ZoneId = 1, Active = true }
            };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(zonesMock);
            _mapperMock.Setup(m => m.Map<List<EvacuationZoneResponse>>(It.IsAny<List<EvacuationZoneEntity>>()))
                .Returns((List<EvacuationZoneEntity> entities) => 
                    entities.Select(e => new EvacuationZoneResponse
                    {
                        ZoneId = e.ZoneId
                    }).ToList());

            // Act
            var result = await _zoneService.GetActiveEvacuationZonesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.GetAllActiveAsync(), Times.Once());
        }

        [Fact]
        public async Task GetActiveEvacuationZonesAsync_ReturnsEmpty_WhenNoActiveZonesExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.GetAllActiveAsync()).ReturnsAsync(new List<EvacuationZoneEntity>());
            _mapperMock.Setup(m => m.Map<List<EvacuationZoneResponse>>(It.IsAny<List<EvacuationZoneEntity>>()))
                .Returns((List<EvacuationZoneEntity> entities) => 
                    entities.Select(e => new EvacuationZoneResponse
                    {
                        ZoneId = e.ZoneId
                    }).ToList());

            // Act
            var result = await _zoneService.GetActiveEvacuationZonesAsync();

            // Assert
            Assert.Empty(result);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.GetAllActiveAsync(), Times.Once());
        }

        [Fact]
        public async Task GetEvacuationZoneByIdAsync_ReturnsZone_WhenFound()
        {
            // Arrange
            var zoneMock = new EvacuationZoneEntity { ZoneId = 1 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(zoneMock.ZoneId)).ReturnsAsync(zoneMock);
            _mapperMock.Setup(m => m.Map<EvacuationZoneResponse>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity e) => new EvacuationZoneResponse
                {
                    ZoneId = e.ZoneId
                });

            // Act
            var result = await _zoneService.GetEvacuationZoneByIdAsync(zoneMock.ZoneId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(zoneMock.ZoneId, result.ZoneId);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.FindByIdAsync(zoneMock.ZoneId), Times.Once);
        }

        [Fact]
        public async Task GetEvacuationZoneByIdAsync_ThrowsArgumentException_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((EvacuationZoneEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _zoneService.GetEvacuationZoneByIdAsync(0));
            Assert.Equal($"EvacuationZone not found.", ex.Message);
        }

        [Fact]
        public async Task CreateEvacuationZoneAsync_ReturnsCreatedZone()
        {
            // Arrange
            var request = new EvacuationZoneRequest { NumberOfPeople = 20, UrgencyLevel = 5 };

            _mapperMock.Setup(m => m.Map<EvacuationZoneEntity>(It.IsAny<EvacuationZoneRequest>()))
                .Returns((EvacuationZoneRequest req) => new EvacuationZoneEntity
                 {
                     NumberOfPeople = req.NumberOfPeople,
                     UrgencyLevel = (ZoneUrgencyLevel)req.UrgencyLevel
                 });

            _unitOfWorkMock.Setup(u => u.EvacuationZones.AddAsync(It.IsAny<EvacuationZoneEntity>())).ReturnsAsync((EvacuationZoneEntity e) => e);

            _mapperMock.Setup(m => m.Map<EvacuationZoneResponse>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity e) => new EvacuationZoneResponse
                {
                    ZoneId = e.ZoneId,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    NumberOfPeople = e.NumberOfPeople,
                    UrgencyLevel = (int)e.UrgencyLevel
                });

            // Act
            var result = await _zoneService.CreateEvacuationZoneAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.NumberOfPeople, result.NumberOfPeople);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.AddAsync(It.IsAny<EvacuationZoneEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateEvacuationZoneAsync_UpdatesZone_WhenFound()
        {
            // Arrange
            var request = new EvacuationZoneRequest { NumberOfPeople = 20 };
            var zoneMock = new EvacuationZoneEntity { ZoneId = 1, NumberOfPeople = 10 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(zoneMock.ZoneId)).ReturnsAsync(zoneMock);

            _unitOfWorkMock.Setup(u => u.EvacuationZones.Update(It.IsAny<EvacuationZoneEntity>())).Returns((EvacuationZoneEntity e) => e);

            _mapperMock.Setup(m => m.Map<EvacuationZoneResponse>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity e) => new EvacuationZoneResponse
                {
                    ZoneId = e.ZoneId,
                    NumberOfPeople = e.NumberOfPeople
                });

            // Act
            var result = await _zoneService.UpdateEvacuationZoneAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.NumberOfPeople, result.NumberOfPeople);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.Update(It.IsAny<EvacuationZoneEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateEvacuationZoneAsync_ThrowsArgumentException_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((EvacuationZoneEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _zoneService.UpdateEvacuationZoneAsync(It.IsAny<int>(), It.IsAny<EvacuationZoneRequest>()));
            Assert.Equal($"EvacuationZone not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteEvacuationZoneAsync_RemovesZone_WhenFound()
        {
            // Arrange
            var zoneMock = new EvacuationZoneEntity { ZoneId = 1, NumberOfPeople = 10 };

            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(zoneMock.ZoneId)).ReturnsAsync(zoneMock);

            _unitOfWorkMock.Setup(u => u.EvacuationZones.Remove(It.IsAny<EvacuationZoneEntity>())).Returns((EvacuationZoneEntity e) => e);

            _mapperMock.Setup(m => m.Map<EvacuationZoneResponse>(It.IsAny<EvacuationZoneEntity>()))
                .Returns((EvacuationZoneEntity e) => new EvacuationZoneResponse
                {
                    ZoneId = e.ZoneId,
                    NumberOfPeople = e.NumberOfPeople     
                });

            // Act
            var result = await _zoneService.DeleteEvacuationZoneAsync(zoneMock.ZoneId);

            // Assert
            Assert.NotNull(result);
            _unitOfWorkMock.Verify(u => u.EvacuationZones.Remove(It.IsAny<EvacuationZoneEntity>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteEvacuationZoneAsync_ThrowsArgumentException_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.EvacuationZones.FindByIdAsync(It.IsAny<int>())).ReturnsAsync((EvacuationZoneEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _zoneService.DeleteEvacuationZoneAsync(It.IsAny<int>()));
            Assert.Equal($"EvacuationZone not found.", ex.Message);
        }
    }
}
