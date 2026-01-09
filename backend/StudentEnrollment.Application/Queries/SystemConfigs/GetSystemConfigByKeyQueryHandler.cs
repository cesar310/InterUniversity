using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.SystemConfigs;

public sealed class GetSystemConfigByKeyQueryHandler(
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<GetSystemConfigByKeyQuery, SystemConfigDto>
{
    public async Task<SystemConfigDto> Handle(
        GetSystemConfigByKeyQuery request,
        CancellationToken cancellationToken)
    {
        var config = await systemConfigRepository.GetByKeyAsync(request.Key, cancellationToken)
            ?? throw new NotFoundException($"Configuración del sistema con clave '{request.Key}' no encontrada", "CONFIG_NOT_FOUND");

        return new SystemConfigDto(
            Id: config.Id,
            ConfigKey: config.ConfigKey,
            ConfigValue: config.ConfigValue,
            ValueType: config.ValueType,
            Description: config.Description,
            IsEditable: config.IsEditable,
            UpdatedAt: config.UpdatedAt
        );
    }
}
