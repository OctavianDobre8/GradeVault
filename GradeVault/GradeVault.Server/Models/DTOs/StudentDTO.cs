namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for student information
     *
     * This DTO represents a student entity with basic identifying information
     * for display and reference in the application.
     */
    public class StudentDTO
    {
        /**
         * @brief Gets or sets the unique identifier for the student
         */
        public int Id { get; set; }
        
        /**
         * @brief Gets or sets the student's first name
         */
        public string FirstName { get; set; }
        
        /**
         * @brief Gets or sets the student's last name
         */
        public string LastName { get; set; }
        
        /**
         * @brief Gets or sets the student's email address
         */
        public string Email { get; set; }
    }
}