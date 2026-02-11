using System;
using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room Number is required.")]
        [StringLength(20, ErrorMessage = "Room Number cannot be longer than 20 characters.")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Room Type is required.")]
        [StringLength(50, ErrorMessage = "Room Type cannot be longer than 50 characters.")]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
    }
}
