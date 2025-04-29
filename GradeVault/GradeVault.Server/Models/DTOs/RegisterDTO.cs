using System.ComponentModel.DataAnnotations;

namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for user registration
     *
     * This DTO captures all necessary information to create a new user account
     * in the GradeVault system, including validation constraints.
     */
    public class RegisterDTO
    {
        /**
         * @brief Email address of the user registering
         * 
         * Must be a valid email format and is required for account creation.
         * Used as the primary means of communication and account identification.
         */
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        /**
         * @brief First name of the user
         * 
         * Must be at least 4 characters long and can only contain letters.
         */
        [Required]
        [MinLength(4, ErrorMessage = "First name must be at least 4 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters")]
        public string? FirstName { get; set; }

        /**
         * @brief Last name of the user
         * 
         * Must be at least 4 characters long and can only contain letters.
         */
        [Required]
        [MinLength(4, ErrorMessage = "Last name must be at least 4 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters")]
        public string? LastName { get; set; }

        /**
         * @brief Password for the new user account
         * 
         * Must be at least 8 characters long for security purposes.
         */
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        /**
         * @brief Password confirmation to prevent typos
         * 
         * Must match the Password field exactly.
         */
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        /**
         * @brief User role in the system
         * 
         * Must be either 'Teacher' or 'Student' to determine access permissions.
         */
        [Required]
        [RegularExpression("^(Teacher|Student)$", ErrorMessage = "Role must be either 'Teacher' or 'Student'")]
        public string? Role { get; set; }  //Student or Teacher
    }
}