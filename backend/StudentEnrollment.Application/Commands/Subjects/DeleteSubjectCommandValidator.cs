using FluentValidation;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class DeleteSubjectCommandValidator : AbstractValidator<DeleteSubjectCommand>
{
    public DeleteSubjectCommandValidator()
    {
        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("El ID de la materia debe ser mayor que 0");
    }
}
