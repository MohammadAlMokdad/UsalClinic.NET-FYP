using System;

namespace UsalClinic.Application.Models
{
    public class NurseDto
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public int YearsOfExperience { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
