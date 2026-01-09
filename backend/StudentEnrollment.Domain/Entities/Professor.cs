namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Profesor (NO es usuario del sistema)
/// Solo los administradores gestionan profesores
/// </summary>
public sealed class Professor
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Specialization { get; set; }
    
    public string? Email { get; set; }
    
    public string? Phone { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    
    public int? CreatedBy { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
