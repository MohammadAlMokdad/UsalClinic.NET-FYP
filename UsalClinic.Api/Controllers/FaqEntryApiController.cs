using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;

namespace UsalClinic.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FaqEntryApiController : ControllerBase
    {
        private readonly FAQEntryService _faqService;

        public FaqEntryApiController(FAQEntryService faqService)
        {
            _faqService = faqService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FAQEntryDto>>> GetAll()
        {
            var faqs = await _faqService.GetAllFAQEntriesAsync();
            return Ok(faqs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FAQEntryDto>> GetById(int id)
        {
            var faq = await _faqService.GetFAQEntryByIdAsync(id);
            if (faq == null)
                return NotFound();

            return Ok(faq);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] FAQEntryDto dto)
        {
            var created = await _faqService.CreateFAQEntryAsync(dto);
            return Ok(new { message = "FAQ created successfully.", id = created.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] FAQEntryDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched ID.");

            var updated = await _faqService.UpdateFAQEntryAsync(id, dto);
            if (updated == null)
                return NotFound();

            return Ok(new { message = "FAQ updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _faqService.DeleteFAQEntryAsync(id);
            if (!success)
                return NotFound();

            return Ok(new { message = "FAQ deleted successfully." });
        }
    }
}
