namespace GradeVault.Server.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int Value { get; set; } // e.g., 1-10
        public DateTime DateAssigned { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }
        public int ClassId { get; set; }

        // Relationships
        public Student Student { get; set; }
        public Class Class { get; set; }
    }

}