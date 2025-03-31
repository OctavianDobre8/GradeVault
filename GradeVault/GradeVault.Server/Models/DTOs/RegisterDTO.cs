namespace GradeVault.Server.Models.DTOs
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }  //Student or Teacher
    }
}
