using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class DoctorDepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        [Display(Name = "Doctor")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Assigned At")]
        [DataType(DataType.DateTime)]
        public DateTime AssignedAt { get; set; }
    }
}
