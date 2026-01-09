using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Configuración dinámica del sistema
/// </summary>
public sealed class SystemConfig
{
    public int Id { get; set; }
    
    public string ConfigKey { get; set; } = string.Empty;
    
    public string ConfigValue { get; set; } = string.Empty;
    
    public ConfigValueType ValueType { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsEditable { get; set; } = true;
    
    public int? UpdatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
