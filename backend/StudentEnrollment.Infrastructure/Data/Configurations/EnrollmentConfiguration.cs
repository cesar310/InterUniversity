using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Infrastructure.Data.Configurations;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        // Composite Primary Key
        builder.HasKey(e => new { e.StudentId, e.SubjectId });
        
        builder.Property(e => e.StudentId)
            .HasColumnName("student_id");

        builder.Property(e => e.SubjectId)
            .HasColumnName("subject_id");

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EnrollmentStatus.Active);

        builder.Property(e => e.EnrolledAt)
            .HasColumnName("enrolled_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(e => e.Status);
    }
}
