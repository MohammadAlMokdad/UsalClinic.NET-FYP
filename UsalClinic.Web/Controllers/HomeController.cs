using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using UsalClinic.Application.Models;
using UsalClinic.Core.Entities;
using UsalClinic.Application.Interfaces;
using UsalClinic.Web.ViewModels;

namespace UsalClinic.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // If user is logged in, redirect to role-specific page
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (await _userManager.IsInRoleAsync(user, "Doctor"))
                {
                    return RedirectToAction("Index", "User");
                }
                else if (await _userManager.IsInRoleAsync(user, "Patient"))
                {
                    return RedirectToAction("Index", "User");
                }
                else if (await _userManager.IsInRoleAsync(user, "Nurse"))
                {
                    return RedirectToAction("Index", "User");
                }
            }

            // Public home page (contact form)
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return the public home page with validation errors
                return View("Index", model);
            }

            // Prepare the email body
            var emailBody = new StringBuilder();
            emailBody.AppendLine($"Name: {model.Name}");
            emailBody.AppendLine($"Email: {model.Email}");
            emailBody.AppendLine("Message:");
            emailBody.AppendLine(model.Comment);

            try
            {
                await _emailService.SendEmailAsync(
                    toEmail: "dans16999@gmail.com",
                    subject: "New Contact Form Submission",
                    body: emailBody.ToString());

                TempData["SuccessMessage"] = "Thank you for contacting us. We will get back to you soon.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Sorry, there was a problem sending your message. Please try again later.";
                return View("Index", model);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            // If the request is coming from AJAX (for dashboard dynamic loading)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_PrivacyPartial"); // Return only the inner content
            }

            // Normal request (direct URL navigation)
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
