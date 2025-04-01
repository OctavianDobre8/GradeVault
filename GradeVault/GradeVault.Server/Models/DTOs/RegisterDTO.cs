using System.ComponentModel.DataAnnotations;

namespace GradeVault.Server.Models.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "First name must be at least 4 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters")]
        public string? FirstName { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Last name must be at least 4 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters")]
        public string? LastName { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("^(Teacher|Student)$", ErrorMessage = "Role must be either 'Teacher' or 'Student'")]
        public string? Role { get; set; }  //Student or Teacher
    }
}
