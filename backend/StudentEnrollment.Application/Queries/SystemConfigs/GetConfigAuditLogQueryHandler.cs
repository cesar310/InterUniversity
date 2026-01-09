using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Queries.SystemConfigs;

public sealed class GetConfigAuditLogQueryHandler(
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<GetConfigAuditLogQuery, IEnumerable<ConfigAuditDto>>
{
    public async Task<IEnumerable<ConfigAuditDto>> Handle(
        GetConfigAuditLogQuery request,
        CancellationToken cancellationToken)
    {
        var auditLog = await systemConfigRepository.GetConfigAuditLogAsync(cancellationToken);
        
        return auditLog.Select(item => new ConfigAuditDto(
            Id: item.Id,
            ConfigKey: item.ConfigKey,
            OldValue: item.OldValue,
            NewValue: item.NewValue,
            ChangedBy: item.ChangedBy,
            ChangedAt: item.ChangedAt
        )).ToList();
    }
}
