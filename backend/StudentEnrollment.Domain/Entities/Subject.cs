namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Materia/Asignatura
/// </summary>
public sealed class Subject
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int Credits { get; set; } = 3;
    
    public int ProfessorId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Professor Professor { get; set; } = null!;
    
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
