using System.ComponentModel.DataAnnotations;

namespace GradeVault.Server.Models.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}