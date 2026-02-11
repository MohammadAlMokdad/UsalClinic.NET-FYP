using System.ComponentModel.DataAnnotations;
using UsalClinic.Web.Validation;

namespace UsalClinic.Web.ViewModels
{
    public class PatientRequestViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of Birth must be in the past")]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        public string Major { get; set; }

        public string BloodType { get; set; }
    }

}
