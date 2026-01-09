using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Students;

public sealed class UpdateStudentCommandHandler(
    IStudentRepository studentRepository)
    : IRequestHandler<UpdateStudentCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateStudentCommand request,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        student.Name = request.Name;
        student.StudentCode = request.StudentCode;

        await studentRepository.UpdateAsync(student, cancellationToken);

        return Unit.Value;
    }
}
