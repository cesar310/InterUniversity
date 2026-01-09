using FluentValidation;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class CreateProfessorCommandValidator : AbstractValidator<CreateProfessorCommand>
{
    public CreateProfessorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del profesor es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre del profesor no puede exceder 100 caracteres");

        When(x => !string.IsNullOrEmpty(x.Specialization), () =>
        {
            RuleFor(x => x.Specialization)
                .MaximumLength(100)
                .WithMessage("La especialización no puede exceder 100 caracteres");
        });

        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("El email no es válido")
                .MaximumLength(100)
                .WithMessage("El email no puede exceder 100 caracteres");
        });

        When(x => !string.IsNullOrEmpty(x.Phone), () =>
        {
            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres");
        });
    }
}
