using System;

namespace UsalClinic.Application.Models
{
    public class FAQEntryDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
