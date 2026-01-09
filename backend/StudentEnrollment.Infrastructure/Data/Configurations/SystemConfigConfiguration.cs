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
                ConfigKey = "default_credits",
                ConfigValue = "3",
                ValueType = ConfigValueType.Int,
                Description = "Créditos por defecto para nuevas materias",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SystemConfig
            {
                Id = 4,
                ConfigKey = "system_mode",
                ConfigValue = "production",
                ValueType = ConfigValueType.String,
                Description = "Modo del sistema (production/maintenance)",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
