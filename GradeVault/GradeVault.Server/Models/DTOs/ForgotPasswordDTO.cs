using System.ComponentModel.DataAnnotations;

namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for password recovery
     *
     * This DTO is used to transfer the email information needed
     * to initiate a password recovery process for a user account.
     */
    public class ForgotPasswordDTO
    {
        /**
         * @brief Email address of the user requesting password recovery
         * 
         * This property is required and must be a valid email format.
         * The system uses this email to send password reset instructions.
         */
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}