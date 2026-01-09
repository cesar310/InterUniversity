using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Infrastructure.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Many-to-Many relationship con Users
        builder.HasMany(r => r.Users)
            .WithMany(u => u.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("user_id", "role_id");
                    j.ToTable("user_roles");
                    j.Property<DateTime>("assigned_at")
                        .HasColumnName("assigned_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                });

        // Data Seeding para roles iniciales
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "administrator",
                Description = "Gestiona configuración, profesores, materias",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = 2,
                Name = "student",
                Description = "Se inscribe en materias",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
