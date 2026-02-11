using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsalClinic.Application.Models
{
    public class PatientRequestDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string Major { get; set; }

        public string BloodType { get; set; }

        public DateTime RequestedAt { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }
    }

}
