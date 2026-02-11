using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;


namespace UsalClinic.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool MustChangePassword { get; set; } = true; // Default true for new accounts
        public string FullName { get; set; }
        public Patient PatientProfile { get; set; }
        public Doctor DoctorProfile { get; set; }
        public Nurse NurseProfile { get; set; }
    }
}
