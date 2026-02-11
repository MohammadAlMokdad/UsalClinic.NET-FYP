using AspnetRun.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsalClinic.Core.Entities
{
    public class Nurse : Entity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public int YearsOfExperience { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // If Nurses have related entities (like schedules, assignments), add collections here
        // e.g., public ICollection<Shift> Shifts { get; set; }
    }
}
