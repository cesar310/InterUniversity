using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.SystemConfigs;

public sealed class UpdateSystemConfigCommandHandler(
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<UpdateSystemConfigCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateSystemConfigCommand request,
        CancellationToken cancellationToken)
    {
        var config = await systemConfigRepository.GetByKeyAsync(request.Key, cancellationToken)
            ?? throw new NotFoundException($"Configuración del sistema con clave '{request.Key}' no encontrada", "CONFIG_NOT_FOUND");

        if (!config.IsEditable)
        {
            throw new BusinessRuleException(
                $"La configuración '{request.Key}' no es editable",
                "CONFIG_NOT_EDITABLE"
            );
        }

        // Validar el tipo de valor según el tipo de configuración
        if (!ValidateValueType(request.Value, config.ValueType))
        {
            throw new DomainValidationException(
                $"Tipo de valor inválido para la configuración '{request.Key}'. Se esperaba {config.ValueType}",
                "INVALID_VALUE_TYPE"
            );
        }

        config.ConfigValue = request.Value;
        config.UpdatedBy = request.UpdatedBy;
        config.UpdatedAt = DateTime.UtcNow;

        await systemConfigRepository.UpdateAsync(config, cancellationToken);

        return Unit.Value;
    }

    private static bool ValidateValueType(string value, Domain.Enums.ConfigValueType expectedType)
    {
        return expectedType switch
        {
            Domain.Enums.ConfigValueType.Int => int.TryParse(value, out _),
            Domain.Enums.ConfigValueType.Boolean => bool.TryParse(value, out _),
            Domain.Enums.ConfigValueType.Decimal => decimal.TryParse(value, out _),
            Domain.Enums.ConfigValueType.String => true,
            _ => false
        };
    }
}
