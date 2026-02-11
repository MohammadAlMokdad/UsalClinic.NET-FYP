namespace UsalClinic.Application.Models
{
    public class PatientDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string Major { get; set; }

        public string BloodType { get; set; }
    }
}