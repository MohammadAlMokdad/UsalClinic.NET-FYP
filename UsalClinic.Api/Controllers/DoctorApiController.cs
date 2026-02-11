using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;

namespace UsalClinic.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorApiController : ControllerBase
    {
        private readonly DoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorApiController(
            DoctorService doctorService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _doctorService = doctorService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetById(Guid id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] DoctorDto dto)
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

            var result = await _userManager.CreateAsync(newUser, "U@u123456");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(newUser, "Doctor");

            dto.UserId = newUser.Id;
            dto.UserName = newUser.UserName;

            await _doctorService.CreateDoctorAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(Guid id, [FromBody] DoctorDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Doctor ID mismatch.");

            var existing = await _doctorService.GetDoctorByIdAsync(id);
            if (existing == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user == null)
                return NotFound("Associated user not found.");

            if (user.FullName != dto.FullName)
            {
                user.FullName = dto.FullName;
                var userUpdateResult = await _userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded)
                    return BadRequest(userUpdateResult.Errors);
            }

            await _doctorService.UpdateDoctorAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            await _doctorService.DeleteDoctorAsync(id);
            return NoContent();
        }
    }
}
