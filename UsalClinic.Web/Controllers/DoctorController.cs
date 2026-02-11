using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(
            DoctorService doctorService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _doctorService = doctorService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Doctor,Admin,Patient,Nurse")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            var viewModels = _mapper.Map<IEnumerable<DoctorViewModel>>(doctors);
            return View(viewModels);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Failed to create doctor. Please check the form.";
                TempData["ToastType"] = "error";
                return View(model);
            }

            var normalized = model.FullName.Replace(" ", "").ToLower();
            var generatedEmail = $"{normalized}@clinic.com";

            var newUser = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = generatedEmail,
                Email = generatedEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, "U@u123456");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                TempData["ToastMessage"] = "Failed to create user account for doctor.";
                TempData["ToastType"] = "error";
                return View(model);
            }

            await _userManager.AddToRoleAsync(newUser, "Doctor");

            var doctorDto = _mapper.Map<DoctorDto>(model);
            doctorDto.UserId = newUser.Id;
            doctorDto.UserName = newUser.UserName;

            try
            {
                await _doctorService.CreateDoctorAsync(doctorDto);

                TempData["ToastMessage"] = "Doctor created successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while creating the doctor.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var doctorDto = await _doctorService.GetDoctorByIdAsync(id);
            if (doctorDto == null)
            {
                TempData["ToastMessage"] = "Doctor not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<DoctorViewModel>(doctorDto);
            return PartialView("_Edit", viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Failed to update doctor. Please check the form.";
                TempData["ToastType"] = "error";
                return PartialView("_Edit", model);
            }

            var existingDto = await _doctorService.GetDoctorByIdAsync(model.Id);
            if (existingDto == null)
            {
                TempData["ToastMessage"] = "Doctor not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(existingDto.UserId);
            if (user == null)
            {
                TempData["ToastMessage"] = "Associated user not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            if (user.FullName != model.FullName)
            {
                user.FullName = model.FullName;
                var userUpdateResult = await _userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded)
                {
                    foreach (var error in userUpdateResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    TempData["ToastMessage"] = "Failed to update user info.";
                    TempData["ToastType"] = "error";
                    return PartialView("_Edit", model);
                }
            }

            try
            {
                var updatedDto = _mapper.Map<DoctorDto>(model);
                await _doctorService.UpdateDoctorAsync(updatedDto);

                TempData["ToastMessage"] = "Doctor updated successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while updating the doctor.";
                TempData["ToastType"] = "error";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                TempData["ToastMessage"] = "Doctor not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _doctorService.DeleteDoctorAsync(id);

                TempData["ToastMessage"] = "Doctor deleted successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while deleting the doctor.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
