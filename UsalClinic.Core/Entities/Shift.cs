using AspnetRun.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsalClinic.Core.Entities
{
    public class Shift : Entity
    {
        public int Id { get; set; }

        public string StaffId { get; set; }  
        public ApplicationUser Staff { get; set; } 

        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsRepeatingWeekly { get; set; }
        public string Role { get; set; } 
    }

}
