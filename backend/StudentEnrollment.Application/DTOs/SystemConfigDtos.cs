using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de configuración del sistema
/// </summary>
public sealed record SystemConfigDto(
    int Id,
    string ConfigKey,
    string ConfigValue,
    ConfigValueType ValueType,
    string? Description,
    bool IsEditable,
    DateTime UpdatedAt
);
