namespace UsalClinic.Application.Models
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int? AppointmentId { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}