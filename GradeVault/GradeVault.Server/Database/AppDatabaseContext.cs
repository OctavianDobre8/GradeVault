using GradeVault.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Database context for the GradeVault application.
/// </summary>
/// <remarks>
/// This class extends IdentityDbContext to provide both identity management 
/// and application data storage for the GradeVault system.
/// </remarks>
public class AppDatabaseContext : IdentityDbContext<User>
{
    /// <summary>
    /// Initializes a new instance of the AppDatabaseContext class.
    /// </summary>
    /// <param name="options">Configuration options for Entity Framework.</param>
    public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options)
        : base(options)
    { }

    /// <summary>
    /// Gets or sets the users collection in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// Gets or sets the students collection in the database.
    /// </summary>
    public DbSet<Student> Students { get; set; }
    
    /// <summary>
    /// Gets or sets the teachers collection in the database.
    /// </summary>
    public DbSet<Teacher> Teachers { get; set; }
    
    /// <summary>
    /// Gets or sets the classes collection in the database.
    /// </summary>
    public DbSet<Class> Classes { get; set; }
    
    /// <summary>
    /// Gets or sets the grades collection in the database.
    /// </summary>
    public DbSet<Grade> Grades { get; set; }
    
    /// <summary>
    /// Gets or sets the class enrollments collection in the database.
    /// </summary>
    public DbSet<ClassEnrollment> ClassEnrollments { get; set; }

    /// <summary>
    /// Configures the database model and entity relationships.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configure composite primary key for ClassEnrollment
    modelBuilder.Entity<ClassEnrollment>()
        .HasKey(ce => new { ce.StudentId, ce.ClassId });
    
    // Map to the Enrollments table
    modelBuilder.Entity<ClassEnrollment>().ToTable("Enrollments");
}
}