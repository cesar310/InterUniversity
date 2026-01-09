using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class DeleteSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IEnrollmentRepository enrollmentRepository)
    : IRequestHandler<DeleteSubjectCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteSubjectCommand request,
        CancellationToken cancellationToken)
    {
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        // Validar que no tenga inscripciones activas
        var hasActiveEnrollments = await enrollmentRepository.HasActiveEnrollmentsBySubjectIdAsync(request.SubjectId, cancellationToken);

        if (hasActiveEnrollments)
        {
            throw new BusinessRuleException(
                "No se puede eliminar una materia con inscripciones activas",
                "SUBJECT_HAS_ACTIVE_ENROLLMENTS"
            );
        }

        // Eliminar la materia
        await subjectRepository.DeleteAsync(request.SubjectId, cancellationToken);

        return Unit.Value;
    }
}
