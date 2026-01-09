using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Infrastructure.Data.Configurations;

public sealed class ConfigAuditLogConfiguration : IEntityTypeConfiguration<ConfigAuditLog>
{
    public void Configure(EntityTypeBuilder<ConfigAuditLog> builder)
    {
        builder.ToTable("config_audit_log");

        builder.HasKey(cal => cal.Id);
        
        builder.Property(cal => cal.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(cal => cal.ConfigKey)
            .HasColumnName("config_key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cal => cal.OldValue)
            .HasColumnName("old_value")
            .HasMaxLength(255);

        builder.Property(cal => cal.NewValue)
            .HasColumnName("new_value")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cal => cal.ChangedBy)
            .HasColumnName("changed_by");

        builder.Property(cal => cal.ChangedAt)
            .HasColumnName("changed_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(cal => cal.ConfigKey);
        builder.HasIndex(cal => cal.ChangedBy);
        builder.HasIndex(cal => cal.ChangedAt);

        // Relación con User (ChangedBy)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(cal => cal.ChangedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
