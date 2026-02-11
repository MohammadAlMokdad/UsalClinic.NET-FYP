namespace UsalClinic.Application.Models
{
    public class DoctorDto
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Profession { get; set; }

        public int YearsOfExperience { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
