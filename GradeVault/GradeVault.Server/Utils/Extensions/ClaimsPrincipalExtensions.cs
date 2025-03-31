using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GradeVault.Server.Utils.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                   principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        }

        public static string? GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email) ??
                   principal.FindFirstValue(JwtRegisteredClaimNames.Email);
        }

        public static string? GetUserFirstName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("FirstName");
        }

        public static string? GetUserLastName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("LastName");
        }

        public static bool IsTeacher(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Teacher");
        }

        public static bool IsStudent(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Student");
        }
    }
}
