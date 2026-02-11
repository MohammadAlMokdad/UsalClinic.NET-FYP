using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        [Display(Name = "Doctor")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status can't be longer than 50 characters.")]
        public string Status { get; set; }

        [StringLength(500, ErrorMessage = "Notes can't be longer than 500 characters.")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        public IEnumerable<SelectListItem>? Doctors { get; set; }
        public IEnumerable<SelectListItem>? Patients { get; set; }

        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
    }
}
