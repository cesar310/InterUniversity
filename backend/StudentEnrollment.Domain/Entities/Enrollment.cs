using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Inscripción de un estudiante a una materia
/// </summary>
public sealed class Enrollment
{
    public int StudentId { get; set; }
    
    public int SubjectId { get; set; }
    
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    
    public DateTime EnrolledAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    
    public Subject Subject { get; set; } = null!;
}
