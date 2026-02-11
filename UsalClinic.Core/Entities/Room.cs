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


    public class Room : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string RoomNumber { get; set; }

        [Required]
        public string RoomType { get; set; } // e.g., Consultation, Surgery, etc.

        public bool IsAvailable { get; set; } = true; // Room availability status

        [MaxLength(1000)]
        public string Description { get; set; } // Additional room details

        [Required]
        public int DepartmentId { get; set; } // Foreign Key to the Department

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; } // Navigation Property

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
