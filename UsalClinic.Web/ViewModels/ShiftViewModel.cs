using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.ViewModels
{
    public class ShiftViewModel
    {
        public int Id { get; set; }

        [Required]
        public string StaffId { get; set; }

        [Display(Name = "Staff Name")]
        public string ?StaffName { get; set; }

        [Required]
        [Display(Name = "Days of Week")]
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Repeats Weekly")]
        public bool IsRepeatingWeekly { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
