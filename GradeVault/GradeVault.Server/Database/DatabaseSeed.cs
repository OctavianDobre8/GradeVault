using Microsoft.AspNetCore.Identity;

namespace GradeVault.Server.Database
{
    public static class DatabaseSeed
    {
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
