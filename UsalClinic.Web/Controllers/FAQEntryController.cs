using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class FAQEntryController : Controller
    {
        private readonly FAQEntryService _faqEntryService;
        private readonly IMapper _mapper;

        public FAQEntryController(FAQEntryService faqEntryService, IMapper mapper)
        {
            _faqEntryService = faqEntryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var faqs = await _faqEntryService.GetAllFAQEntriesAsync();
            var viewModels = _mapper.Map<IEnumerable<FAQEntryViewModel>>(faqs);
            return View(viewModels);
        }

        [Authorize(Roles = "Doctor,Admin,Patient")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Doctor,Admin,Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Failed to create FAQ. Please check the form.";
                TempData["ToastType"] = "error";
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<FAQEntryDto>(model);
                dto.CreatedAt = DateTime.UtcNow;

                await _faqEntryService.CreateFAQEntryAsync(dto);

                TempData["ToastMessage"] = "FAQ entry created successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while creating the FAQ entry.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var faqDto = await _faqEntryService.GetFAQEntryByIdAsync(id);
            if (faqDto == null)
                return NotFound();

            var viewModel = _mapper.Map<FAQEntryViewModel>(faqDto);
            return PartialView("_Edit", viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FAQEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Failed to update FAQ. Please check the form.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", model);
                }

                return View("Index");
            }

            try
            {
                var dto = _mapper.Map<FAQEntryDto>(model);
                await _faqEntryService.UpdateFAQEntryAsync(dto);

                TempData["ToastMessage"] = "FAQ entry updated successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while updating the FAQ entry.";
                TempData["ToastType"] = "error";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _faqEntryService.DeleteFAQEntryAsync(id);

                TempData["ToastMessage"] = "FAQ entry deleted successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while deleting the FAQ entry.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
