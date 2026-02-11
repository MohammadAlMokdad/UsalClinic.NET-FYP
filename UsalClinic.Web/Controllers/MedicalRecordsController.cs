using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class MedicalRecordsController : Controller
    {
        private readonly MedicalRecordService _medicalRecordService;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;
        private readonly PrescriptionService _prescriptionService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicalRecordsController(
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var records = await _medicalRecordService.GetAllMedicalRecordsAsync();
            var viewModels = _mapper.Map<IEnumerable<MedicalRecordViewModel>>(records);
            return View(viewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Doctors = await GetDoctorsSelectListAsync();
            ViewBag.Patients = await GetPatientsSelectListAsync();
            return View();
        }

        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateMedical(int patientId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(currentUserId);
                if (doctor == null) return Forbid();

                var existing = await _medicalRecordService.GetMedicalRecordByDoctorAndPatientAsync(doctor.Id, patientId);
                if (existing != null)
                    return RedirectToAction(nameof(Details), new { patientId });
            }
            else
            {
                var existing = await _medicalRecordService.GetMedicalRecordByPatientIdAsync(patientId);
                if (existing != null)
                    return RedirectToAction(nameof(Details), new { patientId });
            }

            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null) return NotFound();

            ViewBag.Doctors = await GetDoctorsSelectListAsync();

            var viewModel = new MedicalRecordViewModel
            {
                PatientId = patientId,
                PatientName = patient.FullName
            };

            return View("CreateMedical", viewModel);
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedical(MedicalRecordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await GetDoctorsSelectListAsync();
                ViewBag.Patients = await GetPatientsSelectListAsync();
                TempData["ToastMessage"] = "Failed to create medical record. Please check the form.";
                TempData["ToastType"] = "error";
                return View(viewModel);
            }

            try
            {
                var dto = _mapper.Map<MedicalRecordDto>(viewModel);
                await _medicalRecordService.CreateMedicalRecordAsync(dto);
                TempData["ToastMessage"] = "Medical record created successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while creating the medical record.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Details), new { patientId = viewModel.PatientId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await GetDoctorsSelectListAsync();
                ViewBag.Patients = await GetPatientsSelectListAsync();
                TempData["ToastMessage"] = "Failed to create medical record. Please check the form.";
                TempData["ToastType"] = "error";
                return View(viewModel);
            }

            try
            {
                var dto = _mapper.Map<MedicalRecordDto>(viewModel);
                await _medicalRecordService.CreateMedicalRecordAsync(dto);
                TempData["ToastMessage"] = "Medical record created successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while creating the medical record.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
            if (dto == null) return NotFound();

            var viewModel = _mapper.Map<MedicalRecordViewModel>(dto);
            ViewBag.Doctors = await GetDoctorsSelectListAsync();
            ViewBag.Patients = await GetPatientsSelectListAsync();
            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalRecordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdown lists or any needed data
                ViewBag.Doctors = await GetDoctorsSelectListAsync();
                ViewBag.Patients = await GetPatientsSelectListAsync();

                TempData["ToastMessage"] = "Failed to update medical record. Please check the form.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", viewModel);
                }
                return View("Index");
            }

            try
            {
                var existingRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(viewModel.Id);
                if (existingRecord == null)
                    return NotFound();

                var dto = _mapper.Map<MedicalRecordDto>(viewModel);

                // Preserve CreatedAt or any other fields from existing record
                dto.CreatedAt = existingRecord.CreatedAt;

                await _medicalRecordService.UpdateMedicalRecordAsync(dto);

                TempData["ToastMessage"] = "Medical record updated successfully.";
                TempData["ToastType"] = "success";

                // Return JSON success to AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while updating the medical record.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", viewModel);
                }
                return View("Index");
            }
        }


        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _medicalRecordService.DeleteMedicalRecordAsync(id);
                TempData["ToastMessage"] = "Medical record deleted successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while deleting the medical record.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetDoctorsSelectListAsync()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return doctors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Profession
            });
        }

        private async Task<IEnumerable<SelectListItem>> GetPatientsSelectListAsync()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return patients.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FullName
            });
        }

        [Authorize(Roles = "Admin,Doctor,Patient,Nurse")]
        public async Task<IActionResult> Details(int patientId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var records = await _medicalRecordService.GetMedicalRecordsByPatientAsync(patientId);

            if (records == null || !records.Any())
            {
                if (User.IsInRole("Doctor") || User.IsInRole("Admin") || User.IsInRole("Nurse"))
                {
                    var patient = await _patientService.GetPatientByIdAsync(patientId);
                    if (patient == null) return NotFound();

                    ViewBag.HasRecord = false;
                    ViewBag.PatientName = patient.FullName;
                    ViewBag.PatientId = patientId;
                    return View();
                }

                return Forbid();
            }

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(currentUserId);
                if (doctor == null) return Forbid();

                var record = records.FirstOrDefault(r => r.DoctorId == doctor.Id);
                if (record == null)
                    return RedirectToAction("CreateMedical", new { patientId });

                var viewModel = _mapper.Map<MedicalRecordViewModel>(record);
                var prescriptions = await _prescriptionService.GetPrescriptionsByMedicalRecordIdAsync(record.Id);

                ViewBag.Prescriptions = prescriptions;
                ViewBag.MedicalRecordId = record.Id;
                ViewBag.PatientId = patientId;
                return View(viewModel);
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _patientService.GetPatientByIdAsync(patientId);
                if (patient == null || patient.UserId != currentUserId) return Forbid();

                var latestRecord = records.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                if (latestRecord == null) return Forbid();

                var viewModel = _mapper.Map<MedicalRecordViewModel>(latestRecord);
                var prescriptions = await _prescriptionService.GetPrescriptionsByMedicalRecordIdAsync(latestRecord.Id);

                ViewBag.Prescriptions = prescriptions;
                ViewBag.MedicalRecordId = latestRecord.Id;
                ViewBag.PatientId = patientId;
                return View(viewModel);
            }

            // Admin
            var recent = records.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            var vm = _mapper.Map<MedicalRecordViewModel>(recent);
            var adminPrescriptions = await _prescriptionService.GetPrescriptionsByMedicalRecordIdAsync(recent.Id);

            ViewBag.Prescriptions = adminPrescriptions;
            ViewBag.MedicalRecordId = recent.Id;
            ViewBag.PatientId = patientId;
            return View(vm);
        }
    }
}
