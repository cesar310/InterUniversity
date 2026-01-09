using MediatR;

namespace StudentEnrollment.Application.Commands.SystemConfigs;

public sealed record UpdateSystemConfigCommand(
    string Key,
    string Value,
    int UpdatedBy
) : IRequest<Unit>;
