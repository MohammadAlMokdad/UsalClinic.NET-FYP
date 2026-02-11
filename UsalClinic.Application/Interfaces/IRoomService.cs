using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;  // Assuming RoomDto is in this namespace

namespace UsalClinic.Application.Interfaces
{
    public interface IRoomService
    {
        Task<RoomDto> CreateRoomAsync(RoomDto roomDto);
        Task<RoomDto> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<RoomDto>> GetRoomsByDepartmentAsync(int departmentId);
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto> UpdateRoomAsync(RoomDto roomDto);
        Task<bool> DeleteRoomAsync(int roomId);
        Task<bool> CheckRoomAvailabilityAsync(int roomId);
    }
}
