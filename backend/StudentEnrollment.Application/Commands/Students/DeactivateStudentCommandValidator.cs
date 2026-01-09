using FluentValidation;

namespace StudentEnrollment.Application.Commands.Students;

public sealed class DeactivateStudentCommandValidator : AbstractValidator<DeactivateStudentCommand>
{
    public DeactivateStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("El ID del estudiante debe ser mayor que 0");
    }
}
