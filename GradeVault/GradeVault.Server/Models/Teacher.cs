using System.Security.Claims;
namespace GradeVault.Server.Models
{
    /**
     * @brief Entity class representing a teacher in the system
     *
     * This model represents a teacher with identifying information
     * and relationships to classes they teach.
     */
    public class Teacher
    {
        /**
         * @brief Unique identifier for the teacher
         */
        public int Id { get; set; }
        
        /**
         * @brief Teacher's first name
         */
        public string? FirstName { get; set; }
        
        /**
         * @brief Teacher's last name
         */
        public string? LastName { get; set; }
        
        /**
         * @brief Teacher's email address for communication
         */
        public string? Email { get; set; }

        /**
         * @brief Foreign key reference to the user account associated with this teacher
         */
        public string? UserId { get; set; }

        // Relationships
        /**
         * @brief Collection of classes taught by this teacher
         */
        public ICollection<Class>? Classes { get; set; }
    }
}