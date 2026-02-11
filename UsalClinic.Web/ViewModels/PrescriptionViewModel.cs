using System;
using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Medical Record is required.")]
        [Display(Name = "Medical Record")]
        public int MedicalRecordId { get; set; }

        // Used for redirecting to patient's medical record details
        public int patientId { get; set; }

        [Required(ErrorMessage = "Medication Name is required.")]
        [MaxLength(255, ErrorMessage = "Medication Name cannot exceed 255 characters.")]
        [Display(Name = "Medication Name")]
        public string MedicationName { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        [MaxLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        [MaxLength(100, ErrorMessage = "Frequency cannot exceed 100 characters.")]
        public string Frequency { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [MaxLength(100, ErrorMessage = "Duration cannot exceed 100 characters.")]
        public string Duration { get; set; }

        [MaxLength(1000, ErrorMessage = "Instructions cannot exceed 1000 characters.")]
        public string Instructions { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
