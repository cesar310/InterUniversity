using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Infrastructure.Data.Configurations;

public sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");

        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasColumnType("TEXT")
            .IsRequired(false);

        builder.Property(s => s.Credits)
            .HasColumnName("credits")
            .HasDefaultValue(3)
            .IsRequired();

        builder.Property(s => s.ProfessorId)
            .HasColumnName("professor_id")
            .IsRequired();

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.HasIndex(s => s.ProfessorId)
            .HasDatabaseName("idx_professor");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("idx_active");

        // Relationships
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Subject)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
