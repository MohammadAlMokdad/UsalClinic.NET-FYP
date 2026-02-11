using UsalClinic.Application.Models;

namespace UsalClinic.Web.ViewModels
{
    public class UserLandingViewModel
    {
        public string Fullname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public List<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    }

}
