using FluentValidation;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class DeactivateProfessorCommandValidator : AbstractValidator<DeactivateProfessorCommand>
{
    public DeactivateProfessorCommandValidator()
    {
        RuleFor(x => x.ProfessorId)
            .GreaterThan(0)
            .WithMessage("El ID del profesor debe ser mayor que 0");
    }
}
