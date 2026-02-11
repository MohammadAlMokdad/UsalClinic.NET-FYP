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
    [Authorize(Roles = "Doctor,Admin")]
    public class PatientApiController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientApiController(
            PatientService patientService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _patientService = patientService;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/PatientApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            IEnumerable<PatientDto> patients;

            if (isAdmin)
                patients = await _patientService.GetAllPatientsAsync();
            else
                patients = await _patientService.GetPatientsByDoctorUserIdAsync(currentUser.Id);

            return Ok(patients);
        }

        // GET: api/PatientApi/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // POST: api/PatientApi
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var normalized = dto.FullName.Replace(" ", "").ToLower();
            var generatedEmail = $"{normalized}@clinic.com";

            var newUser = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = generatedEmail,
                Email = generatedEmail,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(newUser, "U@u123456");

            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);

            var roleResult = await _userManager.AddToRoleAsync(newUser, "Patient");
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            dto.UserId = newUser.Id;
            dto.UserName = newUser.UserName;

            await _patientService.CreatePatientAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // PUT: api/PatientApi/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingDto = await _patientService.GetPatientByIdAsync(id);
            if (existingDto == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(existingDto.UserId);
            if (user == null)
                return NotFound("Associated user not found");

            if (user.FullName != dto.FullName)
            {
                user.FullName = dto.FullName;
                var userUpdateResult = await _userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded)
                    return BadRequest(userUpdateResult.Errors);
            }

            dto.UserId = existingDto.UserId;
            dto.FullName = user.FullName;

            await _patientService.UpdatePatientAsync(dto);
            return NoContent();
        }

        // DELETE: api/PatientApi/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();

            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }
    }
}
