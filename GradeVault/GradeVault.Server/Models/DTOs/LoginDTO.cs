using System.ComponentModel.DataAnnotations;

namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for user authentication
     *
     * This DTO is used to transfer login credentials from client to server
     * during the authentication process.
     */
    public class LoginDTO
    {
        /**
         * @brief Email address of the user attempting to log in
         * 
         * This property is required and must be in valid email format.
         * It serves as the unique identifier for the user account.
         */
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /**
         * @brief Password for user authentication
         * 
         * This property is required and contains the user's password.
         * It should be transmitted securely and never stored in plain text.
         */
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        /**
         * @brief Flag indicating whether to persist the login session
         * 
         * When true, the server should create a persistent authentication
         * cookie that survives browser restarts.
         */
        public bool RememberMe { get; set; }    
    }
}