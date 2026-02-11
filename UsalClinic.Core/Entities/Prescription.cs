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


    public class Prescription : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicalRecordId { get; set; }

        [ForeignKey("MedicalRecordId")]
        public MedicalRecord MedicalRecord { get; set; }

        [Required]
        [MaxLength(255)]
        public string MedicationName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Duration { get; set; } 

        [MaxLength(1000)]
        public string Instructions { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
