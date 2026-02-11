using System;
using System.ComponentModel.DataAnnotations;
using UsalClinic.Web.Validation;

namespace UsalClinic.Web.ViewModels
{
    public class PatientViewModel
    {
        public int? Id { get; set; }

        public string? UserName { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [PastDate(ErrorMessage = "Date of Birth must be in the past")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be Male, Female")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "Major cannot exceed 100 characters")]
        public string Major { get; set; }

        [Display(Name = "Blood Type")]
        public string BloodType { get; set; }
    }
}
