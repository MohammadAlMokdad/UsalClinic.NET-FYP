namespace UsalClinic.Application.Models
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        public Guid DoctorId { get; set; }
        public string DoctorName {  get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}