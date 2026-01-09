using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.SystemConfigs;

public sealed record GetAllSystemConfigsQuery() : IRequest<IEnumerable<SystemConfigDto>>;
