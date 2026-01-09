using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Students;

public sealed class DeleteStudentCommandHandler(
    IStudentRepository studentRepository,
    IUserRepository userRepository)
    : IRequestHandler<DeleteStudentCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteStudentCommand request,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        var user = await userRepository.GetByIdAsync(student.UserId, cancellationToken)
            ?? throw new NotFoundException($"Usuario del estudiante no encontrado", "USER_NOT_FOUND");

        // Eliminar el estudiante (esto debería eliminar automáticamente las inscripciones por cascada)
        await studentRepository.DeleteAsync(request.StudentId, cancellationToken);
        
        // Eliminar el usuario asociado
        await userRepository.DeleteAsync(student.UserId, cancellationToken);

        return Unit.Value;
    }
}
