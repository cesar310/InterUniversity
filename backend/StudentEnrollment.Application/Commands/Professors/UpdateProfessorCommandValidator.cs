using FluentValidation;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class UpdateProfessorCommandValidator : AbstractValidator<UpdateProfessorCommand>
{
    public UpdateProfessorCommandValidator()
    {
        RuleFor(x => x.ProfessorId)
            .GreaterThan(0)
            .WithMessage("El ID del profesor debe ser mayor que 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del profesor es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre del profesor no puede exceder 100 caracteres");

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("La especialización es requerida")
            .MaximumLength(100)
            .WithMessage("La especialización no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .EmailAddress()
            .WithMessage("El email no es válido")
            .MaximumLength(100)
            .WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("El teléfono es requerido")
            .MaximumLength(20)
            .WithMessage("El teléfono no puede exceder 20 caracteres");
    }
}
