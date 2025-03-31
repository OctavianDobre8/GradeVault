using Microsoft.AspNetCore.Identity;

namespace GradeVault.Server.Models
{
    public class User: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
    }
}
