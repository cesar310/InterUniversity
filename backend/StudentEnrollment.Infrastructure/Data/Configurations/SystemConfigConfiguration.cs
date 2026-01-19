using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Infrastructure.Data.Configurations;

public sealed class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
{
    public void Configure(EntityTypeBuilder<SystemConfig> builder)
    {
        builder.ToTable("system_config");

        builder.HasKey(sc => sc.Id);
        
        builder.Property(sc => sc.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(sc => sc.ConfigKey)
            .HasColumnName("config_key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sc => sc.ConfigValue)
            .HasColumnName("config_value")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(sc => sc.ValueType)
            .HasColumnName("value_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(sc => sc.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(sc => sc.IsEditable)
            .HasColumnName("is_editable")
            .HasDefaultValue(true);

        builder.Property(sc => sc.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(sc => sc.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(sc => sc.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(sc => sc.ConfigKey)
            .IsUnique();
        
        // Relación con User (UpdatedBy)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sc => sc.UpdatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Data Seeding para configuraciones iniciales
        builder.HasData(
            new SystemConfig
            {
                Id = 1,
                ConfigKey = "max_subjects_per_student",
                ConfigValue = "3",
                ValueType = ConfigValueType.Int,
                Description = "Máximo de materias que un estudiante puede inscribir simultáneamente",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 2,
                ConfigKey = "max_subjects_per_professor",
                ConfigValue = "3",
                ValueType = ConfigValueType.Int,
                Description = "Máximo de materias que un profesor puede dictar",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 3,
                ConfigKey = "default_subject_credits",
                ConfigValue = "3",
                ValueType = ConfigValueType.Int,
                Description = "Créditos predeterminados por materia",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 4,
                ConfigKey = "min_subjects_per_student",
                ConfigValue = "1",
                ValueType = ConfigValueType.Int,
                Description = "Mínimo de materias que un estudiante debe inscribir",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 5,
                ConfigKey = "allow_same_professor",
                ConfigValue = "false",
                ValueType = ConfigValueType.Boolean,
                Description = "Permitir que un estudiante tome múltiples materias del mismo profesor",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 6,
                ConfigKey = "system_name",
                ConfigValue = "Sistema de Inscripción Estudiantil",
                ValueType = ConfigValueType.String,
                Description = "Nombre del sistema",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 7,
                ConfigKey = "academic_period",
                ConfigValue = "2026-1",
                ValueType = ConfigValueType.String,
                Description = "Período académico actual",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 8,
                ConfigKey = "enrollment_open",
                ConfigValue = "true",
                ValueType = ConfigValueType.Boolean,
                Description = "Indica si las inscripciones están abiertas",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
