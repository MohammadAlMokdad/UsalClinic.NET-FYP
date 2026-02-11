namespace UsalClinic.Application.Models
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int patientId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}