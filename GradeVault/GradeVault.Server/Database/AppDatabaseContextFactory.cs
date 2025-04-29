using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GradeVault.Server.Database
{
    /// <summary>
    /// Factory class for creating instances of AppDatabaseContext during design time.
    /// </summary>
    /// <remarks>
    /// This factory is used by Entity Framework Core tools like migrations
    /// to create instances of the database context outside of the application's
    /// normal dependency injection container.
    /// </remarks>
    public class AppDatabaseContextFactory : IDesignTimeDbContextFactory<AppDatabaseContext>
    {
        /// <summary>
        /// Creates a new instance of AppDatabaseContext for design-time services.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>An instance of AppDatabaseContext configured with connection settings from appsettings.json.</returns>
        /// <remarks>
        /// This method is called by Entity Framework tools like Add-Migration, Update-Database, etc.
        /// It reads the database connection string from appsettings.json and configures 
        /// the database context accordingly.
        /// </remarks>
        public AppDatabaseContext CreateDbContext(string[] args)
        {
            // Load configuration from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get the actual connection string value
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDatabaseContext>();
            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 36)));

            return new AppDatabaseContext(optionsBuilder.Options);
        }
    }
}