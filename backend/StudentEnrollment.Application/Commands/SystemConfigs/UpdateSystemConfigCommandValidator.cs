using FluentValidation;

namespace StudentEnrollment.Application.Commands.SystemConfigs;

public sealed class UpdateSystemConfigCommandValidator : AbstractValidator<UpdateSystemConfigCommand>
{
    public UpdateSystemConfigCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("La clave de configuración es requerida")
            .MaximumLength(100)
            .WithMessage("La clave de configuración no puede exceder 100 caracteres");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("El valor de configuración es requerido")
            .MaximumLength(255)
            .WithMessage("El valor de configuración no puede exceder 255 caracteres");

        RuleFor(x => x.UpdatedBy)
            .GreaterThan(0)
            .WithMessage("El ID de quien actualiza debe ser mayor que 0");
    }
}
