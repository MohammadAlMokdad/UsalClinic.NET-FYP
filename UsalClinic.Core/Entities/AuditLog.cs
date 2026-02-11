using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AspnetRun.Core.Entities.Base;


namespace UsalClinic.Core.Entities
{

    public class AuditLog : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Action { get; set; } 

        [Required]
        public string EntityName { get; set; } 

        [Required]
        public string EntityId { get; set; } 

        [MaxLength(1000)]
        public string Details { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string PerformedBy { get; set; } 
    }
}
