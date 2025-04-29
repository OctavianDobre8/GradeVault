namespace GradeVault.Server.Models.DTOs
{
    public class ClassDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } // Was Subject
        public string RoomNumber { get; set; }
        public string TeacherName { get; set; }
    }

    public class CreateClassDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RoomNumber { get; set; }
    }
}