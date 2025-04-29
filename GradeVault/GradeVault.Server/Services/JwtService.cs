using GradeVault.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GradeVault.Server.Services
{
    /**
     * @brief Service for generating JSON Web Tokens (JWT) for authentication
     *
     * This service is responsible for creating secure JWTs that contain user identity information
     * and are used for authenticating API requests in the GradeVault application.
     */
    public class JwtService
    {
        /**
         * @brief Configuration service for accessing application settings
         */
        private readonly IConfiguration _configuration;

        /**
         * @brief Constructor that initializes the JWT service with application configuration
         *
         * @param configuration The application configuration containing JWT settings
         */
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /**
         * @brief Generates a JWT token for the specified user with their roles
         *
         * Creates a secure token containing claims about the user's identity, including
         * their ID, email, name, and assigned roles in the system.
         *
         * @param user The user for whom to generate the token
         * @param roles List of roles assigned to the user
         * @return String representation of the generated JWT token
         * @throws ArgumentNullException If the JWT key is not configured
         */
        public string GenerateToken(User user, IList<string> roles)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("Jwt:Key", "JWT key cannot be null or empty.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}