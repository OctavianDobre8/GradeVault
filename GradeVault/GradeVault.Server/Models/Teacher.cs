using System.Security.Claims;
namespace GradeVault.Server.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Relationships
        public ICollection<Class> Classes { get; set; }
    }
}

