namespace GradeVault.Server.Models
{
    public class ClassEnrollment
    {
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }

        // Relationships
        public Student? Student { get; set; }
        public Class? Class { get; set; }
    }

}