using FluentValidation;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("El ID de la materia debe ser mayor que 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre de la materia es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre de la materia no puede exceder 100 caracteres");

        RuleFor(x => x.Credits)
            .GreaterThan(0)
            .WithMessage("Los créditos deben ser mayores que 0")
            .LessThanOrEqualTo(10)
            .WithMessage("Los créditos no pueden exceder 10");

        RuleFor(x => x.ProfessorId)
            .GreaterThan(0)
            .WithMessage("El ID del profesor debe ser mayor que 0");
    }
}
