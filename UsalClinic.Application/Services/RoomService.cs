using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;

namespace UsalClinic.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository, IMapper mapper, ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RoomDto> CreateRoomAsync(RoomDto roomDto)
        {
            _logger.LogInformation("Creating room in department ID: {DepartmentId}", roomDto.DepartmentId);
            var room = _mapper.Map<Room>(roomDto);
            room.CreatedAt = DateTime.UtcNow;
            var newRoom = await _roomRepository.AddAsync(room);
            _logger.LogInformation("Room created with ID: {RoomId}", newRoom.Id);
            return _mapper.Map<RoomDto>(newRoom);
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int roomId)
        {
            _logger.LogInformation("Fetching room with ID: {RoomId}", roomId);
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                _logger.LogWarning("Room with ID: {RoomId} not found", roomId);
                return null;
            }
            return _mapper.Map<RoomDto>(room);
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsByDepartmentAsync(int departmentId)
        {
            _logger.LogInformation("Fetching rooms for department ID: {DepartmentId}", departmentId);
            var rooms = await _roomRepository.GetByDepartmentIdAsync(departmentId);
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            _logger.LogInformation("Fetching all rooms");
            var rooms = await _roomRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<RoomDto> UpdateRoomAsync(RoomDto roomDto)
        {
            _logger.LogInformation("Updating room with ID: {RoomId}", roomDto.Id);
            var existingRoom = await _roomRepository.GetByIdAsync(roomDto.Id);
            if (existingRoom == null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found for update", roomDto.Id);
                throw new ApplicationException($"Room with Id {roomDto.Id} not found.");
            }

            // Preserve CreatedAt before mapping
            var originalCreatedAt = existingRoom.CreatedAt;

            _mapper.Map(roomDto, existingRoom);

            // Re-assign original CreatedAt
            existingRoom.CreatedAt = originalCreatedAt;

            await _roomRepository.UpdateAsync(existingRoom);
            _logger.LogInformation("Room with ID: {RoomId} updated successfully", roomDto.Id);
            return _mapper.Map<RoomDto>(existingRoom);
        }


        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            _logger.LogInformation("Deleting room with ID: {RoomId}", roomId);
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found for deletion", roomId);
                throw new ApplicationException($"Room with Id {roomId} not found.");
            }

            await _roomRepository.DeleteAsync(room);
            _logger.LogInformation("Room with ID: {RoomId} deleted successfully", roomId);
            return true;
        }

        public async Task<bool> CheckRoomAvailabilityAsync(int roomId)
        {
            _logger.LogInformation("Checking availability for room ID: {RoomId}", roomId);
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found when checking availability", roomId);
                throw new ApplicationException($"Room with Id {roomId} not found.");
            }

            _logger.LogInformation("Room ID: {RoomId} is {Availability}", roomId, room.IsAvailable ? "available" : "unavailable");
            return room.IsAvailable;
        }
    }
}
