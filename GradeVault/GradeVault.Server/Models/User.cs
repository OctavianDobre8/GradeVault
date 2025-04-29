using Microsoft.AspNetCore.Identity;

namespace GradeVault.Server.Models
{
    /**
     * @brief Custom user entity extending ASP.NET Identity's base user
     *
     * This class extends the standard IdentityUser with additional properties
     * necessary for the GradeVault application's user management and display requirements.
     */
    public class User: IdentityUser
    {
        /**
         * @brief User's first name for display and identification purposes
         */
        public string? FirstName { get; set; }
        
        /**
         * @brief User's last name for display and identification purposes
         */
        public string? LastName { get; set; }
        
        /**
         * @brief User's role in the system (e.g., "Teacher", "Student")
         *
         * Determines the user's permissions and available features in the application.
         */
        public string? Role { get; set; }
    }
}