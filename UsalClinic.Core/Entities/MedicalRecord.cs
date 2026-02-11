using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspnetRun.Core.Entities.Base;

namespace UsalClinic.Core.Entities
{


    public class MedicalRecord : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }

        public int? AppointmentId { get; set; } 

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Diagnosis { get; set; }

        [MaxLength(1000)]
        public string Prescription { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Adding collection for prescriptions
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
