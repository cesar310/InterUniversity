namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Rol del sistema (administrator, student)
/// </summary>
public sealed class Role
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
}
