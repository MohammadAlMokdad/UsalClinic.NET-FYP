using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;

namespace UsalClinic.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomApiController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomApiController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetByDepartment(int departmentId)
        {
            var rooms = await _roomService.GetRoomsByDepartmentAsync(departmentId);
            return Ok(rooms);
        }

        [HttpGet("{id}/availability")]
        public async Task<ActionResult<bool>> CheckAvailability(int id)
        {
            var isAvailable = await _roomService.CheckRoomAvailabilityAsync(id);
            return Ok(isAvailable);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RoomDto dto)
        {
            var created = await _roomService.CreateRoomAsync(dto);
            return Ok(new { message = "Room created successfully.", id = created.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] RoomDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched ID.");

            await _roomService.UpdateRoomAsync(dto);
            return Ok(new { message = "Room updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _roomService.DeleteRoomAsync(id);
            return Ok(new { message = "Room deleted successfully." });
        }
    }
}
