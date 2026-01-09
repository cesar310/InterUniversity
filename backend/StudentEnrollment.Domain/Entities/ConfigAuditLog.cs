namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Auditoría de cambios en configuraciones del sistema
/// </summary>
public sealed class ConfigAuditLog
{
    public int Id { get; set; }
    
    public string ConfigKey { get; set; } = string.Empty;
    
    public string? OldValue { get; set; }
    
    public string NewValue { get; set; } = string.Empty;
    
    public int? ChangedBy { get; set; }
    
    public DateTime ChangedAt { get; set; }
}
