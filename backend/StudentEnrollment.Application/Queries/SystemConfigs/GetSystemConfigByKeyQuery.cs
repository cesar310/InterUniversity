using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.SystemConfigs;

public sealed record GetSystemConfigByKeyQuery(string Key) : IRequest<SystemConfigDto>;
