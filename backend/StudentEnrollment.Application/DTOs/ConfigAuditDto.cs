namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de auditoría de configuración desde view_config_audit
/// </summary>
public sealed record ConfigAuditDto(
    int Id,
    string ConfigKey,
    string? OldValue,
    string NewValue,
    string? ChangedBy,
    DateTime ChangedAt
);
