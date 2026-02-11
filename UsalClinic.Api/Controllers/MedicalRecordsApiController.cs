using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;

namespace UsalClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsApiController : ControllerBase
    {
        private readonly MedicalRecordService _medicalRecordService;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;
        private readonly PrescriptionService _prescriptionService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicalRecordsApiController(
            MedicalRecordService medicalRecordService,
            DoctorService doctorService,
            PatientService patientService,
            PrescriptionService prescriptionService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _patientService = patientService;
            _prescriptionService = prescriptionService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetAll()
        {
            var records = await _medicalRecordService.GetAllMedicalRecordsAsync();
            return Ok(records);
        }

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor,Admin,Patient")]
        public async Task<ActionResult<MedicalRecordDto>> GetByPatientId(int patientId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var records = await _medicalRecordService.GetMedicalRecordsByPatientAsync(patientId);

            if (records == null || !records.Any())
                return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(currentUserId);
                if (doctor == null)
                    return Forbid();

                var record = records.FirstOrDefault(r => r.DoctorId == doctor.Id);
                if (record == null)
                    return Forbid();

                return Ok(record);
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _patientService.GetPatientByIdAsync(patientId);
                if (patient == null || patient.UserId != currentUserId)
                    return Forbid();

                var latestRecord = records.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                return Ok(latestRecord);
            }

            // Admin
            var adminRecord = records.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            return Ok(adminRecord);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _medicalRecordService.CreateMedicalRecordAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPost("doctor")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateMedical([FromBody] MedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(currentUserId);
                if (doctor == null)
                    return Forbid();

                var existing = await _medicalRecordService.GetMedicalRecordByDoctorAndPatientAsync(doctor.Id, dto.PatientId);
                if (existing != null)
                    return Conflict("Medical record already exists for this doctor and patient.");

                dto.DoctorId = doctor.Id;
            }

            await _medicalRecordService.CreateMedicalRecordAsync(dto);
            return Ok(dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalRecordDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            await _medicalRecordService.UpdateMedicalRecordAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _medicalRecordService.DeleteMedicalRecordAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<ActionResult<MedicalRecordDto>> GetById(int id)
        {
            var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
            if (record == null)
                return NotFound();

            return Ok(record);
        }

        [HttpGet("prescriptions/{recordId}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptions(int recordId)
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByMedicalRecordIdAsync(recordId);
            return Ok(prescriptions);
        }
    }
}
