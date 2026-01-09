using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Students;

public sealed class DeactivateStudentCommandHandler(
    IStudentRepository studentRepository,
    IUserRepository userRepository)
    : IRequestHandler<DeactivateStudentCommand, Unit>
{
    public async Task<Unit> Handle(
        DeactivateStudentCommand request,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        var user = await userRepository.GetByIdAsync(student.UserId, cancellationToken)
            ?? throw new NotFoundException($"Usuario del estudiante no encontrado", "USER_NOT_FOUND");

        // Deactivate user account
        user.IsActive = false;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Unit.Value;
    }
}
