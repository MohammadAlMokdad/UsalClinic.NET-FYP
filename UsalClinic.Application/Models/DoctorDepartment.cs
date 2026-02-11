namespace UsalClinic.Application.Models
{
    public class DoctorDepartmentDto
    {
        public int Id { get; set; }

        public Guid DoctorId { get; set; }

        public int DepartmentId { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
