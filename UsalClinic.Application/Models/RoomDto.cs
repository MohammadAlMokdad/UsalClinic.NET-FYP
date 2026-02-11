namespace UsalClinic.Application.Models
{
    public class RoomDto
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; }

        public string RoomType { get; set; }

        public bool IsAvailable { get; set; }

        public string Description { get; set; }

        public int DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}