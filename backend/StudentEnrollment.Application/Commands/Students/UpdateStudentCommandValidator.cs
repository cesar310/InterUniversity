using FluentValidation;

namespace StudentEnrollment.Application.Commands.Students;

public sealed class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("El ID del estudiante debe ser mayor que 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .MaximumLength(200)
            .WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.StudentCode)
            .NotEmpty()
            .WithMessage("El código de estudiante es requerido")
            .MaximumLength(20)
            .WithMessage("El código de estudiante no puede exceder 20 caracteres");
    }
}
