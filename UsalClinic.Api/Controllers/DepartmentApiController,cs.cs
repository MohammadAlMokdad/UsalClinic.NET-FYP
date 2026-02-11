using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;

namespace UsalClinic.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentApiController : ControllerBase
    {
        private readonly DepartmentService _departmentService;

        public DepartmentApiController(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();
            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentDto dto)
        {
            await _departmentService.AddDepartmentAsync(dto);
            return Ok(new { message = "Department created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched ID.");

            await _departmentService.UpdateDepartmentAsync(dto);
            return Ok(new { message = "Department updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return Ok(new { message = "Department deleted successfully." });
        }
    }
}
