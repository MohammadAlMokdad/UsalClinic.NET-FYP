using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class FAQEntryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question is required.")]
        [StringLength(500, ErrorMessage = "Question cannot exceed 500 characters.")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        [StringLength(2000, ErrorMessage = "Answer cannot exceed 2000 characters.")]
        public string Answer { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
