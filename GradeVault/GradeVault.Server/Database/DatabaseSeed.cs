using Microsoft.AspNetCore.Identity;

namespace GradeVault.Server.Database
{
    /// <summary>
    /// Provides methods for seeding initial data into the GradeVault database.
    /// </summary>
    /// <remarks>
    /// This static class contains methods to initialize the database with 
    /// necessary data like user roles during application startup.
    /// </remarks>
    public static class DatabaseSeed
    {
        /// <summary>
        /// Seeds predefined roles into the ASP.NET Identity system.
        /// </summary>
        /// <param name="roleManager">The role manager service from ASP.NET Identity.</param>
        /// <returns>A task representing the asynchronous seeding operation.</returns>
        /// <remarks>
        /// This method creates the Teacher and Student roles if they don't already exist.
        /// These roles are fundamental to the GradeVault permission system and are used
        /// to control access to various features of the application.
        /// </remarks>
        public static async Task seedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Teacher", "Student" };

            foreach(var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}