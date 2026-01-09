using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Enums;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class SystemConfigRepository(ApplicationDbContext context) : ISystemConfigRepository
{
    public async Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
        => await context.SystemConfigs
            .FirstOrDefaultAsync(sc => sc.ConfigKey == key, cancellationToken);

    public async Task<IEnumerable<SystemConfig>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.SystemConfigs
            .OrderBy(sc => sc.ConfigKey)
            .ToListAsync(cancellationToken);

    public async Task UpdateAsync(SystemConfig config, CancellationToken cancellationToken = default)
    {
        context.SystemConfigs.Update(config);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int?> GetIntValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var config = await GetByKeyAsync(key, cancellationToken);
        if (config?.ValueType != ConfigValueType.Int)
            return null;

        return int.TryParse(config.ConfigValue, out var value) ? value : null;
    }

    public async Task<bool?> GetBoolValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var config = await GetByKeyAsync(key, cancellationToken);
        if (config?.ValueType != ConfigValueType.Boolean)
            return null;

        return bool.TryParse(config.ConfigValue, out var value) ? value : null;
    }

    public async Task<string?> GetStringValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var config = await GetByKeyAsync(key, cancellationToken);
        return config?.ValueType == ConfigValueType.String ? config.ConfigValue : null;
    }

    public async Task<IEnumerable<ConfigAuditItem>> GetConfigAuditLogAsync(CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        
        command.CommandText = @"
            SELECT id, config_key, old_value, new_value, changed_by, changed_at 
            FROM view_config_audit 
            ORDER BY changed_at DESC";
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var result = new List<ConfigAuditItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new ConfigAuditItem(
                Id: reader.GetInt32(reader.GetOrdinal("id")),
                ConfigKey: reader.GetString(reader.GetOrdinal("config_key")),
                OldValue: reader.IsDBNull(reader.GetOrdinal("old_value")) ? null : reader.GetString(reader.GetOrdinal("old_value")),
                NewValue: reader.GetString(reader.GetOrdinal("new_value")),
                ChangedBy: reader.IsDBNull(reader.GetOrdinal("changed_by")) ? null : reader.GetString(reader.GetOrdinal("changed_by")),
                ChangedAt: reader.GetDateTime(reader.GetOrdinal("changed_at"))
            ));
        }
        
        return result;
    }
}
