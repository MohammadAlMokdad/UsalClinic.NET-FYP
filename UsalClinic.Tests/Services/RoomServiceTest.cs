using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using Xunit;

namespace UsalClinic.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _mockRoomRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RoomService>> _mockLogger;
        private readonly RoomService _service;

        public RoomServiceTests()
        {
            _mockLogger = new Mock<ILogger<RoomService>>();
            _mockRoomRepo = new Mock<IRoomRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new RoomService(_mockRoomRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateRoomAsync_ReturnsMappedDto()
        {
            var dto = new RoomDto { Id = 0 };
            var entity = new Room();
            var savedEntity = new Room { Id = 1 };
            var savedDto = new RoomDto { Id = 1 };

            _mockMapper.Setup(m => m.Map<Room>(dto)).Returns(entity);
            _mockRoomRepo.Setup(r => r.AddAsync(entity)).ReturnsAsync(savedEntity);
            _mockMapper.Setup(m => m.Map<RoomDto>(savedEntity)).Returns(savedDto);

            var result = await _service.CreateRoomAsync(dto);

            _mockRoomRepo.Verify(r => r.AddAsync(entity), Times.Once);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ReturnsDto_IfFound()
        {
            var entity = new Room { Id = 1 };
            var dto = new RoomDto { Id = 1 };

            _mockRoomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<RoomDto>(entity)).Returns(dto);

            var result = await _service.GetRoomByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ReturnsNull_IfNotFound()
        {
            _mockRoomRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Room?)null);

            var result = await _service.GetRoomByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRoomsByDepartmentAsync_ReturnsMappedList()
        {
            var entities = new List<Room> { new Room { Id = 1 } };
            var dtos = new List<RoomDto> { new RoomDto { Id = 1 } };

            _mockRoomRepo.Setup(r => r.GetByDepartmentIdAsync(10)).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<RoomDto>>(entities)).Returns(dtos);

            var result = await _service.GetRoomsByDepartmentAsync(10);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllRoomsAsync_ReturnsMappedList()
        {
            var entities = new List<Room> { new Room { Id = 1 } };
            var dtos = new List<RoomDto> { new RoomDto { Id = 1 } };

            _mockRoomRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<RoomDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllRoomsAsync();

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task UpdateRoomAsync_ThrowsException_IfNotFound()
        {
            var dto = new RoomDto { Id = 1 };
            _mockRoomRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Room?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.UpdateRoomAsync(dto));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task UpdateRoomAsync_UpdatesAndReturnsDto()
        {
            var dto = new RoomDto { Id = 1 };
            var existing = new Room { Id = 1 };

            _mockRoomRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existing);
            _mockMapper.Setup(m => m.Map(dto, existing));
            _mockMapper.Setup(m => m.Map<RoomDto>(existing)).Returns(dto);

            var result = await _service.UpdateRoomAsync(dto);

            _mockRoomRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task DeleteRoomAsync_ThrowsException_IfNotFound()
        {
            _mockRoomRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Room?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.DeleteRoomAsync(999));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task DeleteRoomAsync_ReturnsTrue_IfDeleted()
        {
            var existing = new Room { Id = 1 };
            _mockRoomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

            var result = await _service.DeleteRoomAsync(1);

            Assert.True(result);
            _mockRoomRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
        }

        [Fact]
        public async Task CheckRoomAvailabilityAsync_ThrowsException_IfNotFound()
        {
            _mockRoomRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Room?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.CheckRoomAvailabilityAsync(999));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task CheckRoomAvailabilityAsync_ReturnsAvailability()
        {
            var room = new Room { Id = 1, IsAvailable = true };
            _mockRoomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);

            var result = await _service.CheckRoomAvailabilityAsync(1);

            Assert.True(result);
        }
    }
}
