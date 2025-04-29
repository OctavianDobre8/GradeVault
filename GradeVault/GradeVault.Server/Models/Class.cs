namespace GradeVault.Server.Models
{
    /**
     * @brief Entity class representing a course or class in the system
     *
     * This model represents an academic class/course with identifying information
     * and relationships to teachers and student enrollments.
     */
    public class Class
    {
        /**
         * @brief Unique identifier for the class
         */
        public int Id { get; set; }
        
        /**
         * @brief Name of the class/course
         */
        public string Name { get; set; }
        
        /**
         * @brief Description of the class content and objectives
         *
         * Was previously named "Subject" in earlier versions.
         */
        public string Description { get; set; } // Was Subject
        
        /**
         * @brief Physical location where the class is held
         */
        public string RoomNumber { get; set; }
        
        /**
         * @brief Foreign key reference to the teacher responsible for this class
         */
        public int TeacherId { get; set; }

        /**
         * @brief Navigation property to the associated teacher
         */
        public Teacher Teacher { get; set; }
        
        /**
         * @brief Collection of student enrollments for this class
         */
        public ICollection<ClassEnrollment> Enrollments { get; set; }
    }
}