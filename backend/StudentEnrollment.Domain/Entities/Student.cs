namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Estudiante del sistema
/// </summary>
public sealed class Student
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string StudentCode { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public int? CreatedBy { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
