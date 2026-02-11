using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;
        private readonly IMapper _mapper;

        public AppointmentController(
            AppointmentService appointmentService,
            DoctorService doctorService,
            PatientService patientService,
            IMapper mapper)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            IEnumerable<AppointmentDto> appointments;

            if (User.IsInRole("Admin") || User.IsInRole("Nurse"))
            {
                appointments = await _appointmentService.GetAllAppointmentsAsync();
            }
            else if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                if (doctor == null)
                    return Forbid();

                appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctor.Id);
            }
            else if (User.IsInRole("Patient"))
            {
                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                if (patient == null)
                    return Forbid();

                appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patient.Id);
            }
            else
            {
                return Forbid();
            }

            var viewModels = _mapper.Map<IEnumerable<AppointmentViewModel>>(appointments);
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new AppointmentViewModel
            {
                Doctors = await GetDoctorsSelectListAsync(),
                Patients = await GetPatientsSelectListAsync(),
                AppointmentDate = DateTime.Now
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Doctors = await GetDoctorsSelectListAsync();
                viewModel.Patients = await GetPatientsSelectListAsync();

                TempData["ToastMessage"] = "Failed to create appointment. Please check the form.";
                TempData["ToastType"] = "error";
                return View(viewModel);
            }

            try
            {
                var dto = _mapper.Map<AppointmentDto>(viewModel);
                await _appointmentService.AddAppointmentAsync(dto);

                TempData["ToastMessage"] = "Appointment created successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while creating the appointment.";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointmentDto == null) return NotFound();

            var viewModel = _mapper.Map<AppointmentViewModel>(appointmentDto);
            viewModel.Doctors = await GetDoctorsSelectListAsync();
            viewModel.Patients = await GetPatientsSelectListAsync();

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Reload select lists
                viewModel.Doctors = await GetDoctorsSelectListAsync();
                viewModel.Patients = await GetPatientsSelectListAsync();

                TempData["ToastMessage"] = "Update failed. Please make sure all fields are filled.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", viewModel);
                }
                return View("Index");
            }

            try
            {
                // Get the existing appointment from DB to preserve CreatedAt
                var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(viewModel.Id);
                if (existingAppointment == null)
                    return NotFound();

                var dto = _mapper.Map<AppointmentDto>(viewModel);

                // Preserve CreatedAt from existing appointment
                dto.CreatedAt = existingAppointment.CreatedAt;

                await _appointmentService.UpdateAppointmentAsync(dto);

                TempData["ToastMessage"] = "Appointment updated successfully.";
                TempData["ToastType"] = "success";

                return Json(new { success = true });
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while updating the appointment.";
                TempData["ToastType"] = "error";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Edit", viewModel);
                }
                return View("Index");
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                TempData["ToastMessage"] = "Appointment not found.";
                TempData["ToastType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);

                TempData["ToastMessage"] = "Appointment deleted successfully.";
                TempData["ToastType"] = "success";
            }
            catch
            {
                TempData["ToastMessage"] = "An error occurred while deleting the appointment.";
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

        [HttpGet]
        public async Task<IActionResult> DetailModal(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<AppointmentViewModel>(appointment);
            return PartialView("_Detail", viewModel);
        }
    }
}
