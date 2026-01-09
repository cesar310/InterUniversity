using FluentValidation;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class DeleteProfessorCommandValidator : AbstractValidator<DeleteProfessorCommand>
{
    public DeleteProfessorCommandValidator()
    {
        RuleFor(x => x.ProfessorId)
            .GreaterThan(0)
            .WithMessage("El ID del profesor debe ser mayor que 0");
    }
}
