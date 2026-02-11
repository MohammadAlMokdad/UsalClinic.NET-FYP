using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;
using UsalClinic.Web.Data;
using UsalClinic.Web.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);

        // Prepare Dashboard Model
        var model = new AdminDashboardViewModel
        {
            AppointmentCount = await _context.Appointments.CountAsync(),
            PatientCount = await _context.Patients.CountAsync(),
            DoctorCount = await _context.Doctors.CountAsync(),
            NurseCount = await _context.Nurses.CountAsync(),
            Departments = await _context.Departments.ToListAsync()
        };

        //  Pass Departments to ViewBag for urgent modal
        ViewBag.Departments = await _context.Departments
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            })
            .ToListAsync();

        //  Pass Calendar Events for FullCalendar
        ViewBag.CalendarEvents = await _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Select(a => new
            {
                id = a.Id,
                title = a.Patient.User.FullName + " - " + a.Doctor.User.FullName,
                start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                description = "Appointment with " + a.Doctor.User.FullName
            })
            .ToListAsync();

        return View(model);
    }

    [HttpGet]
    public IActionResult GetAppointments()
    {
        var events = _context.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Select(a => new
            {
                id = a.Id,
                title = a.Patient.User.FullName + " - " + a.Doctor.User.FullName,
                start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                description = "Appointment with " + a.Doctor.User.FullName
            })
            .ToList();

        return Json(events);
    }
}
