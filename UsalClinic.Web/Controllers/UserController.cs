using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.Http;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Web.ViewModels;

[Authorize]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppointmentService _appointmentService;
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;
    private readonly ShiftService _shiftService;
    private readonly DepartmentService _departmentService;
    private readonly RoomService _roomService;
    private readonly PatientRequestService _patientRequestService;
    private readonly IMapper _mapper;

    public UserController(
        UserManager<ApplicationUser> userManager,
        AppointmentService appointmentService,
        PatientService patientService,
        DoctorService doctorService,
        ShiftService shiftService,
        DepartmentService departmentService,
        RoomService roomService,
        PatientRequestService patientRequestService,
        IMapper mapper)
    {
        _userManager = userManager;
        _appointmentService = appointmentService;
        _patientService = patientService;
        _doctorService = doctorService;
        _shiftService = shiftService;
        _departmentService = departmentService;
        _roomService = roomService;
        _patientRequestService = patientRequestService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }

        var model = new UserLandingViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Fullname = user.FullName
        };

        // Fetch departments and rooms for the modal dropdown
        var departments = await _departmentService.GetAllDepartmentsAsync();
        var rooms = await _roomService.GetAllRoomsAsync();            

        ViewBag.Departments = departments.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.Name
        }).ToList();

        ViewBag.Rooms = rooms.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.RoomNumber.ToString()
        }).ToList();

        // Appointment logic (same as before)
        IEnumerable<AppointmentDto> appointments = new List<AppointmentDto>();

        if (User.IsInRole("Admin"))
        {
            appointments = await _appointmentService.GetAllAppointmentsAsync();
        }
        else if (User.IsInRole("Doctor"))
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(user.Id);
            if (doctor != null)
            {
                appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctor.Id);
            }
        }
        else if (User.IsInRole("Patient"))
        {
            var patient = await _patientService.GetPatientByUserIdAsync(user.Id);
            if (patient != null)
            {
                appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patient.Id);
            }
        }

        var events = appointments.Select(a =>
        {
            string title, tooltip;

            if (User.IsInRole("Admin"))
            {
                title = $"Dr. {a.DoctorName}";
                tooltip = $"Dr. {a.DoctorName} with {a.PatientName} at {a.AppointmentDate:HH:mm}";
            }
            else if (User.IsInRole("Doctor"))
            {
                title = $"Patient: {a.PatientName}";
                tooltip = $"Appointment with {a.PatientName} at {a.AppointmentDate:HH:mm}";
            }
            else
            {
                title = $"Doctor: {a.DoctorName}";
                tooltip = $"Appointment with Dr. {a.DoctorName} at {a.AppointmentDate:HH:mm}";
            }

            return new
            {
                id = a.Id,
                title,
                start = a.AppointmentDate.ToString("s"),
                color = a.AppointmentDate >= DateTime.Now ? "green" : "red",
                description = tooltip
            };
        });

        ViewBag.CalendarEvents = events;

        return View(model);
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    public async Task<IActionResult> SendUrgentAlert(int departmentId, int roomId)
    {
        var userEmail = User?.Identity?.Name; 
        if (string.IsNullOrWhiteSpace(userEmail))
            return Unauthorized();

        var success = await _shiftService.SendUrgentAlertAsync(userEmail, departmentId, roomId);
        if (!success)
            return BadRequest("No available nurse found at this moment.");

        return Ok("Urgent alert sent to available nurse.");
    }
    //Approve Registration Patient Request
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveRequests()
    {
        var requests = await _patientRequestService.GetPendingRequestsAsync();
        return View(requests);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        await _patientRequestService.ApproveAsync(id);
        return RedirectToAction("ApproveRequests");
    }

    [Authorize(Roles = "Admin")]
    [System.Web.Http.HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            await _patientRequestService.RejectAsync(id);
            TempData["ToastMessage"] = "Patient request rejected successfully.";
            TempData["ToastType"] = "success";
        }
        catch (Exception ex)
        {
            TempData["ToastMessage"] = "Failed to reject patient request.";
            TempData["ToastType"] = "error";
        }
        return RedirectToAction("ApproveRequests");
    }

}
