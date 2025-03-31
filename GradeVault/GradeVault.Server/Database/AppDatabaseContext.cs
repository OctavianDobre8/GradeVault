using GradeVault.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDatabaseContext : IdentityDbContext<User>
{
    public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options)
        : base(options)
    { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<ClassEnrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClassEnrollment>()
            .HasKey(ce => new { ce.StudentId, ce.ClassId });
    }

}