using FluentValidation;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed class CancelEnrollmentCommandValidator : AbstractValidator<CancelEnrollmentCommand>
{
    public CancelEnrollmentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("El ID del estudiante debe ser mayor que 0");

        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("El ID de la materia debe ser mayor que 0");
    }
}
