using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShiftController : Controller
    {
        private readonly ShiftService _shiftService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DoctorService _doctorService;
        private readonly NurseService _nurseService;

        public ShiftController(
            ShiftService shiftService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            DoctorService doctorService,
            NurseService nurseService)
        {
            _shiftService = shiftService;
            _mapper = mapper;
            _userManager = userManager;
            _doctorService = doctorService;
            _nurseService = nurseService;
        }

        public async Task<IActionResult> Index()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            var viewModels = _mapper.Map<IEnumerable<ShiftViewModel>>(shifts);
            return View(viewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShiftViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation failed. Please check the form.";
                TempData["ToastType"] = "error";
                return View(model);
            }

            try
            {
                var shiftDto = _mapper.Map<ShiftDto>(model);
                await _shiftService.CreateShiftAsync(shiftDto);

                TempData["ToastMessage"] = "Shift created successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "An unexpected error occurred while creating the shift.";
                TempData["ToastType"] = "error";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var shiftDto = await _shiftService.GetShiftByIdAsync(id);
            if (shiftDto == null)
            {
                TempData["ToastMessage"] = "Shift not found.";
                TempData["ToastType"] = "error";
                return NotFound();
            }

            var viewModel = _mapper.Map<ShiftViewModel>(shiftDto);

            // Load staff list based on Role
            if (viewModel.Role == "Doctor")
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                ViewBag.StaffList = doctors.Select(d => new SelectListItem
                {
                    Value = d.UserId,
                    Text = d.FullName
                }).ToList();
            }
            else if (viewModel.Role == "Nurse")
            {
                var nurses = await _nurseService.GetAllNursesAsync();
                ViewBag.StaffList = nurses.Select(n => new SelectListItem
                {
                    Value = n.UserId,
                    Text = n.FullName
                }).ToList();
            }

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShiftViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation failed while updating shift.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", model);
                }

                return View("Index", model); // or RedirectToAction(nameof(Index));
            }

            try
            {
                var shiftDto = _mapper.Map<ShiftDto>(model);
                await _shiftService.UpdateShiftAsync(model.Id, shiftDto);

                TempData["ToastMessage"] = "Shift updated successfully.";
                TempData["ToastType"] = "success";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "Error occurred during shift update.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", model);
                }

                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var shift = await _shiftService.GetShiftByIdAsync(id);
                if (shift == null)
                {
                    TempData["ToastMessage"] = "Shift not found.";
                    TempData["ToastType"] = "error";
                    return NotFound();
                }

                await _shiftService.DeleteShiftAsync(id);

                TempData["ToastMessage"] = "Shift deleted successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to delete shift.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            var result = doctors.Select(d => new { id = d.UserId, name = d.FullName });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetNurses()
        {
            var nurses = await _nurseService.GetAllNursesAsync();
            var result = nurses.Select(n => new { id = n.UserId, name = n.FullName });
            return Json(result);
        }
    }
}
