using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        [Display(Name = "Doctor")]
        public Guid DoctorId { get; set; }

        public string? DoctorName { get; set; }

        [Display(Name = "Appointment")]
        public int? AppointmentId { get; set; }

        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(1000, ErrorMessage = "Diagnosis cannot exceed 1000 characters.")]
        public string Diagnosis { get; set; }

        [Required(ErrorMessage = "Prescription is required.")]
        [StringLength(1000, ErrorMessage = "Prescription cannot exceed 1000 characters.")]
        public string Prescription { get; set; }

        [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
        public string Notes { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
