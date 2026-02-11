using AspnetRun.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsalClinic.Core.Entities
{
    public class PatientRequest : Entity
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        public string Major { get; set; }

        public string BloodType { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;
    }

}
