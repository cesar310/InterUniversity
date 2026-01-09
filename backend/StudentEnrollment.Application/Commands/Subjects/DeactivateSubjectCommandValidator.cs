using FluentValidation;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class DeactivateSubjectCommandValidator : AbstractValidator<DeactivateSubjectCommand>
{
    public DeactivateSubjectCommandValidator()
    {
        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("El ID de la materia debe ser mayor que 0");
    }
}
