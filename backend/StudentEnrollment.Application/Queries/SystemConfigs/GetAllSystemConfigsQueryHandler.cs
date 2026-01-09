using MediatR;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.SystemConfigs;

public sealed class GetAllSystemConfigsQueryHandler(
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<GetAllSystemConfigsQuery, IEnumerable<SystemConfigDto>>
{
    public async Task<IEnumerable<SystemConfigDto>> Handle(
        GetAllSystemConfigsQuery request,
        CancellationToken cancellationToken)
    {
        var configs = await systemConfigRepository.GetAllAsync(cancellationToken);

        return configs.Select(c => new SystemConfigDto(
            Id: c.Id,
            ConfigKey: c.ConfigKey,
            ConfigValue: c.ConfigValue,
            ValueType: c.ValueType,
            Description: c.Description,
            IsEditable: c.IsEditable,
            UpdatedAt: c.UpdatedAt
        )).ToList();
    }
}
