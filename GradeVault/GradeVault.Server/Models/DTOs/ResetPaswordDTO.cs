using System.ComponentModel.DataAnnotations;

namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for password reset operations
     *
     * This DTO captures all necessary information to reset a user's password,
     * including the email address, reset token, and new password details.
     */
    public class ResetPasswordDTO
    {
        /**
         * @brief Email address of the user requesting password reset
         * 
         * Must be a valid email format and is required to identify the account.
         */
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /**
         * @brief Security token for password reset verification
         * 
         * This single-use token validates that the password reset request is legitimate
         * and was initiated by the account owner.
         */
        [Required]
        public string Token { get; set; }

        /**
         * @brief New password for the user account
         * 
         * Must be at least 8 characters long for security purposes.
         */
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /**
         * @brief Confirmation of the new password
         * 
         * Must match the Password field exactly to prevent typos.
         */
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}