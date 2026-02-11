using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;

namespace UsalClinic.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentApiController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentApiController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AppointmentDto dto)
        {
            await _appointmentService.AddAppointmentAsync(dto);
            return Ok(new { message = "Appointment created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] AppointmentDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched ID.");

            await _appointmentService.UpdateAppointmentAsync(dto);
            return Ok(new { message = "Appointment updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAppointmentAsync(id);
            return Ok(new { message = "Appointment deleted successfully." });
        }

        [HttpGet("by-doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByDoctor(Guid doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            return Ok(appointments);
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatient(int patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            return Ok(appointments);
        }
    }
}
