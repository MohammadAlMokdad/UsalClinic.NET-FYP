using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AspnetRun.Core.Entities.Base;

namespace UsalClinic.Core.Entities
{


    public class Department : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public ICollection<DoctorDepartment> DoctorDepartments { get; set; } 

        
        public ICollection<Room> Rooms { get; set; } 
    }

}
