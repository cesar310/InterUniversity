using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Domain.Interfaces;

public interface ISystemConfigRepository
{
    Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemConfig>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(SystemConfig config, CancellationToken cancellationToken = default);
    Task<int?> GetIntValueAsync(string key, CancellationToken cancellationToken = default);
    Task<bool?> GetBoolValueAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetStringValueAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene el historial de auditoría de configuraciones desde view_config_audit
    /// </summary>
    Task<IEnumerable<ConfigAuditItem>> GetConfigAuditLogAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Item de auditoría de configuración desde view_config_audit
/// </summary>
public sealed record ConfigAuditItem(
    int Id,
    string ConfigKey,
    string? OldValue,
    string NewValue,
    string? ChangedBy,
    DateTime ChangedAt
);
