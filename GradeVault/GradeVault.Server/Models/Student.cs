namespace GradeVault.Server.Models
{
    /**
     * @brief Entity class representing a student in the system
     *
     * This model represents a student with identifying information
     * and relationships to their grades and class enrollments.
     */
    public class Student
    {
        /**
         * @brief Unique identifier for the student
         */
        public int Id { get; set; }
        
        /**
         * @brief Student's first name
         */
        public string? FirstName { get; set; }
        
        /**
         * @brief Student's last name
         */
        public string? LastName { get; set; }
        
        /**
         * @brief Student's email address for communication
         */
        public string? Email { get; set; }

        /**
         * @brief Foreign key reference to the user account associated with this student
         */
        public string? UserId { get; set; }

        // Relationships
        /**
         * @brief Collection of grades received by this student
         */
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        
        /**
         * @brief Collection of class enrollments for this student
         */
        public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
    }
}