using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.SystemConfigs;

/// <summary>
/// Query para obtener auditoría de configuraciones desde view_config_audit
/// </summary>
public sealed record GetConfigAuditLogQuery() : IRequest<IEnumerable<ConfigAuditDto>>;
