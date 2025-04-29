namespace GradeVault.Server.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } // Was Subject
        public string RoomNumber { get; set; }
        
        // Foreign Keys
        public int TeacherId { get; set; }

        // Relationships
        public Teacher Teacher { get; set; }
        public ICollection<ClassEnrollment> Enrollments { get; set; }
    }
}