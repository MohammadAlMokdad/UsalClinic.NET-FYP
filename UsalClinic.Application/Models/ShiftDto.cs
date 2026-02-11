using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsalClinic.Application.Models
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public string StaffId { get; set; }
        public string ?StaffName { get; set; }
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRepeatingWeekly { get; set; }
        public string Role { get; set; }
    }
}
