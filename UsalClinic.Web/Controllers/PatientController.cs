using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly PatientService _patientService;
        private readonly PatientRequestService _patientRequestService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(
            PatientService patientService,
            PatientRequestService patientRequestService,
            IMapper mapper,
            ILogger<PatientController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _patientService = patientService;
            _patientRequestService = patientRequestService;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }
        [Authorize(Roles = "Doctor,Admin,Nurse,Patient")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isNurse = await _userManager.IsInRoleAsync(user, "Nurse");
            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            var isPatient = await _userManager.IsInRoleAsync(user, "Patient");

            IEnumerable<PatientDto> patientsDto;

            if (isAdmin || isNurse)
            {
                patientsDto = await _patientService.GetAllPatientsAsync();
            }
            else if (isDoctor)
            {
                patientsDto = await _patientService.GetPatientsByDoctorUserIdAsync(user.Id);
            }
            else if (isPatient)
            {
                var patient = await _patientService.GetPatientByUserIdAsync(user.Id);
                if (patient == null)
                    return Forbid();

                return RedirectToAction("Details", "MedicalRecords", new { patientId = patient.Id });
            }
            else
            {
                return Forbid();
            }

            var viewModels = _mapper.Map<IEnumerable<PatientViewModel>>(patientsDto);
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
        public async Task<IActionResult> Create(PatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation failed. Please check the form.";
                TempData["ToastType"] = "error";
                return View(model);
            }

            try
            {
                var normalized = model.FullName.Replace(" ", "").ToLower();
                var generatedEmail = $"{normalized}@clinic.com";

                var newUser = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = generatedEmail,
                    Email = generatedEmail,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(newUser, "U@u123456");
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    TempData["ToastMessage"] = "Failed to create patient user.";
                    TempData["ToastType"] = "error";
                    return View(model);
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, "Patient");
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    TempData["ToastMessage"] = "Failed to assign patient role.";
                    TempData["ToastType"] = "error";
                    return View(model);
                }

                var dto = _mapper.Map<PatientDto>(model);
                dto.UserId = newUser.Id;
                dto.UserName = newUser.UserName;

                await _patientService.CreatePatientAsync(dto);

                TempData["ToastMessage"] = "Patient created successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "An unexpected error occurred while creating the patient.";
                TempData["ToastType"] = "error";
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _patientService.GetPatientByIdAsync(id);
            if (dto == null)
                return NotFound();

            var vm = _mapper.Map<PatientViewModel>(dto);
            return PartialView("_EditPartial", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation failed while updating patient.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_EditPartial", model);
                }
                return View("Index", model);
            }

            try
            {
                var existingDto = await _patientService.GetPatientByIdAsync(model.Id!.Value);
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
                            ModelState.AddModelError(string.Empty, error.Description);

                        TempData["ToastMessage"] = "Failed to update user data.";
                        TempData["ToastType"] = "error";

                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return PartialView("_EditPartial", model);
                        }
                        return View("Index", model);
                    }
                }

                var updatedDto = _mapper.Map<PatientDto>(model);
                updatedDto.UserId = existingDto.UserId;
                updatedDto.FullName = user.FullName;

                await _patientService.UpdatePatientAsync(updatedDto);

                TempData["ToastMessage"] = "Patient updated successfully.";
                TempData["ToastType"] = "success";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "Error occurred during update.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_EditPartial", model);
                }
                return View("Index", model);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dto = await _patientService.GetPatientByIdAsync(id);
                if (dto == null)
                {
                    TempData["ToastMessage"] = "Patient not found.";
                    TempData["ToastType"] = "error";
                    return NotFound();
                }

                await _patientService.DeletePatientAsync(id);

                TempData["ToastMessage"] = "Patient deleted successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to delete patient.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        //Request Registration
        [AllowAnonymous]
        public IActionResult RequestAccess()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RequestAccess(PatientRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Generate username/email from FullName
                var normalized = model.FullName.Replace(" ", "").ToLower();
                var generatedEmail = $"{normalized}@clinic.com";

                // Assign the generated email/username to the model
                model.UserName = generatedEmail;

                var dto = _mapper.Map<PatientRequestDto>(model);
                await _patientRequestService.CreateRequestAsync(dto);

                TempData["ToastMessage"] = "Your request has been submitted for admin approval.";
                TempData["ToastType"] = "success";

                // Redirect to UserController's Index action
                return RedirectToAction("Index", "User");
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to submit your request. Please try again.";
                TempData["ToastType"] = "error";
                return View(model);
            }
        }



    }
}
