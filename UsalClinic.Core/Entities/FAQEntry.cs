using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AspnetRun.Core.Entities.Base;


namespace UsalClinic.Core.Entities
{

    public class FAQEntry : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Question { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Answer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
