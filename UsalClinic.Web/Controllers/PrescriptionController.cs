using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly PrescriptionService _prescriptionService;
        private readonly MedicalRecordService _medicalRecordService;
        private readonly IMapper _mapper;

        public PrescriptionController(
            PrescriptionService prescriptionService,
            MedicalRecordService medicalRecordService,
            IMapper mapper)
        {
            _prescriptionService = prescriptionService;
            _medicalRecordService = medicalRecordService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var dtos = await _prescriptionService.GetAllAsync();
            var vms = _mapper.Map<IEnumerable<PrescriptionViewModel>>(dtos);
            return View(vms);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateMedicalRecordsDropDown();
            return View();
        }

        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateForRecord(int medicalRecordId)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(medicalRecordId);
            if (medicalRecord == null)
                return NotFound();

            var vm = new PrescriptionViewModel
            {
                MedicalRecordId = medicalRecordId,
                patientId = medicalRecord.PatientId
            };

            return View("CreateForRecord", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateMedicalRecordsDropDown();
                TempData["ToastMessage"] = "Validation failed. Please check the form.";
                TempData["ToastType"] = "error";
                return View(vm);
            }

            try
            {
                var dto = _mapper.Map<PrescriptionDto>(vm);
                dto.CreatedAt = DateTime.UtcNow;

                await _prescriptionService.CreateAsync(dto);

                TempData["ToastMessage"] = "Prescription created successfully.";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to create prescription.";
                TempData["ToastType"] = "error";
                return View(vm);
            }
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForRecord(PrescriptionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation failed. Please check the form.";
                TempData["ToastType"] = "error";
                return View("CreateForRecord", vm);
            }

            try
            {
                var dto = _mapper.Map<PrescriptionDto>(vm);
                dto.CreatedAt = DateTime.UtcNow;

                await _prescriptionService.CreateAsync(dto);

                var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(dto.MedicalRecordId);
                if (medicalRecord == null)
                    return NotFound();

                TempData["ToastMessage"] = "Prescription added to medical record.";
                TempData["ToastType"] = "success";
                return RedirectToAction("Details", "MedicalRecords", new { patientId = vm.patientId });
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to add prescription.";
                TempData["ToastType"] = "error";
                return View("CreateForRecord", vm);
            }
        }

        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _prescriptionService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            var vm = _mapper.Map<PrescriptionViewModel>(dto);

            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(dto.MedicalRecordId);
            if (medicalRecord != null)
                vm.patientId = medicalRecord.PatientId;

            await PopulateMedicalRecordsDropDown();
            return PartialView("_Edit", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrescriptionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateMedicalRecordsDropDown(); // If needed to reload dropdowns in partial

                TempData["ToastMessage"] = "Validation failed. Please check the form.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", vm);
                }
                return View("Index", vm);
            }

            try
            {
                var dto = _mapper.Map<PrescriptionDto>(vm);
                await _prescriptionService.UpdateAsync(dto);

                TempData["ToastMessage"] = "Prescription updated successfully.";
                TempData["ToastType"] = "success";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to update prescription.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", vm);
                }
                return View("Index", vm);
            }
        }


        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedical(PrescriptionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation failed. Please check the form.";
                TempData["ToastType"] = "error";

                // If AJAX request, return partial view with validation errors
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    await PopulateMedicalRecordsDropDown(); // If dropdowns or ViewBag needed
                    return PartialView("_Edit", vm);
                }

                // If normal post, redirect or show view (adjust as needed)
                return View("Details", vm);
            }

            try
            {
                var dto = _mapper.Map<PrescriptionDto>(vm);
                await _prescriptionService.UpdateAsync(dto);

                TempData["ToastMessage"] = "Prescription updated successfully.";
                TempData["ToastType"] = "success";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Return success JSON for AJAX handler to close modal and refresh
                    return Json(new { success = true });
                }

                return RedirectToAction("Details", "MedicalRecords", new { patientId = vm.patientId });
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to update prescription.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    await PopulateMedicalRecordsDropDown();
                    return PartialView("_Edit", vm);
                }

                return View("Details", vm);
            }
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _prescriptionService.DeleteAsync(id);
                TempData["ToastMessage"] = "Prescription deleted successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to delete prescription.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DeleteMedical(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetByIdAsync(id);
                if (prescription == null)
                    return NotFound();

                var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(prescription.MedicalRecordId);
                if (medicalRecord == null)
                    return NotFound();

                await _prescriptionService.DeleteAsync(id);

                TempData["ToastMessage"] = "Prescription removed from medical record.";
                TempData["ToastType"] = "success";
                return RedirectToAction("Details", "MedicalRecords", new { patientId = medicalRecord.PatientId });
            }
            catch
            {
                TempData["ToastMessage"] = "Failed to remove prescription.";
                TempData["ToastType"] = "error";
                return RedirectToAction("Details", "MedicalRecords", new { patientId = 0 });
            }
        }

        private async Task PopulateMedicalRecordsDropDown()
        {
            var records = await _medicalRecordService.GetAllMedicalRecordsAsync();
            var items = records.Select(r => new
            {
                r.Id,
                Display = $"Record #{r.Id}"
            });

            ViewBag.MedicalRecords = new SelectList(items, "Id", "Display");
        }
    }
}
