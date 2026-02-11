using System;
using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class NurseViewModel
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name can't exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string? UserName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10, ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address can't exceed 255 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Years of experience is required.")]
        [Range(0, 100, ErrorMessage = "Years of experience must be between 0 and 100.")]
        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
