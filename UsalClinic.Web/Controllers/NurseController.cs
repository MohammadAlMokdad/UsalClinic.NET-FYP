using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class NurseController : Controller
    {
        private readonly NurseService _nurseService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public NurseController(
            NurseService nurseService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _nurseService = nurseService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<IActionResult> Index()
        {
            var nurses = await _nurseService.GetAllNursesAsync();
            var viewModels = _mapper.Map<IEnumerable<NurseViewModel>>(nurses);
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
        public async Task<IActionResult> Create(NurseViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
                TempData["ToastMessage"] = "Failed to create nurse user.";
                TempData["ToastType"] = "error";
                return View(model);
            }

            await _userManager.AddToRoleAsync(newUser, "Nurse");

            var nurseDto = _mapper.Map<NurseDto>(model);
            nurseDto.UserId = newUser.Id;
            nurseDto.UserName = newUser.UserName;

            await _nurseService.CreateNurseAsync(nurseDto);

            TempData["ToastMessage"] = "Nurse created successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var nurseDto = await _nurseService.GetNurseByIdAsync(id);
            if (nurseDto == null)
                return NotFound();

            var viewModel = _mapper.Map<NurseViewModel>(nurseDto);
            return PartialView("_Edit", viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NurseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Failed to update nurse. Please check the form.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", model);
                }
                return View("Index", model);
            }

            var existingDto = await _nurseService.GetNurseByIdAsync(model.Id);
            if (existingDto == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(existingDto.UserId);
            if (user == null)
                return NotFound("Associated user not found.");

            if (user.FullName != model.FullName)
            {
                user.FullName = model.FullName;
                var userUpdateResult = await _userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded)
                {
                    foreach (var error in userUpdateResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    TempData["ToastMessage"] = "Failed to update user data.";
                    TempData["ToastType"] = "error";

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return PartialView("_Edit", model);
                    }
                    return View("Index", model);
                }
            }

            var updatedDto = _mapper.Map<NurseDto>(model);
            await _nurseService.UpdateNurseAsync(updatedDto);

            TempData["ToastMessage"] = "Nurse updated successfully.";
            TempData["ToastType"] = "success";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var nurse = await _nurseService.GetNurseByIdAsync(id);
            if (nurse == null)
            {
                TempData["ToastMessage"] = "Nurse not found.";
                TempData["ToastType"] = "error";
                return NotFound();
            }

            await _nurseService.DeleteNurseAsync(id);

            TempData["ToastMessage"] = "Nurse deleted successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}