using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Infrastructure.Data.Configurations;

namespace StudentEnrollment.Infrastructure.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<ConfigAuditLog> ConfigAuditLogs => Set<ConfigAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new ProfessorConfiguration());
        modelBuilder.ApplyConfiguration(new SubjectConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new SystemConfigConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigAuditLogConfiguration());

        // Configurar enum conversions a lowercase para MySQL
        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<Domain.Enums.EnrollmentStatus>(v, true));

        modelBuilder.Entity<SystemConfig>()
            .Property(sc => sc.ValueType)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<Domain.Enums.ConfigValueType>(v, true));
    }
}
