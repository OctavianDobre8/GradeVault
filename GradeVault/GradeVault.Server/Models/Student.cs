namespace GradeVault.Server.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public string? UserId { get; set; }

        // Relationships
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
    }

}
