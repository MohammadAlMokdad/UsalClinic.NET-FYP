using Microsoft.AspNetCore.Mvc;
using UsalClinic.Core.Entities;

namespace UsalClinic.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int AppointmentCount { get; set; }
        public int PatientCount { get; set; }
        public int DoctorCount { get; set; }
        public int NurseCount { get; set; }
        public List<Department> Departments { get; set; } = new List<Department>();

    }
}