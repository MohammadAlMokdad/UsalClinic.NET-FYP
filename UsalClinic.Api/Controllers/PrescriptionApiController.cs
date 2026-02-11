using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;

namespace UsalClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrescriptionApiController : ControllerBase
    {
        private readonly PrescriptionService _prescriptionService;
        private readonly MedicalRecordService _medicalRecordService;
        private readonly IMapper _mapper;

        public PrescriptionApiController(
            PrescriptionService prescriptionService,
            MedicalRecordService medicalRecordService,
            IMapper mapper)
        {
            _prescriptionService = prescriptionService;
            _medicalRecordService = medicalRecordService;
            _mapper = mapper;
        }

        // GET: api/PrescriptionApi
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetAll()
        {
            var prescriptions = await _prescriptionService.GetAllAsync();
            return Ok(prescriptions);
        }

        // GET: api/PrescriptionApi/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<PrescriptionDto>> GetById(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();

            return Ok(prescription);
        }

        // POST: api/PrescriptionApi
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.CreatedAt = DateTime.UtcNow;
            await _prescriptionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // POST: api/PrescriptionApi/ForRecord
        [HttpPost("ForRecord")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateForMedicalRecord([FromBody] PrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var record = await _medicalRecordService.GetMedicalRecordByIdAsync(dto.MedicalRecordId);
            if (record == null)
                return NotFound("Medical record not found.");

            dto.CreatedAt = DateTime.UtcNow;
            await _prescriptionService.CreateAsync(dto);
            return Ok(dto);
        }

        // PUT: api/PrescriptionApi/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] PrescriptionDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            await _prescriptionService.UpdateAsync(dto);
            return NoContent();
        }

        // PUT: api/PrescriptionApi/EditMedical/{id}
        [HttpPut("EditMedical/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateForMedical(int id, [FromBody] PrescriptionDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            await _prescriptionService.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/PrescriptionApi/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();

            await _prescriptionService.DeleteAsync(id);
            return NoContent();
        }

        // DELETE: api/PrescriptionApi/DeleteMedical/{id}
        [HttpDelete("DeleteMedical/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DeleteFromMedicalRecord(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();

            var record = await _medicalRecordService.GetMedicalRecordByIdAsync(prescription.MedicalRecordId);
            if (record == null)
                return NotFound("Related medical record not found.");

            await _prescriptionService.DeleteAsync(id);
            return NoContent();
        }
    }
}
