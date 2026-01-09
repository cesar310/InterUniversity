using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class DeactivateSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IEnrollmentRepository enrollmentRepository)
    : IRequestHandler<DeactivateSubjectCommand, Unit>
{
    public async Task<Unit> Handle(
        DeactivateSubjectCommand request,
        CancellationToken cancellationToken)
    {
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        // Validar que no tenga inscripciones activas
        var hasActiveEnrollments = await enrollmentRepository.HasActiveEnrollmentsBySubjectIdAsync(request.SubjectId, cancellationToken);

        if (hasActiveEnrollments)
        {
            throw new BusinessRuleException(
                "No se puede desactivar una materia con inscripciones activas",
                "SUBJECT_HAS_ACTIVE_ENROLLMENTS"
            );
        }

        subject.IsActive = false;

        await subjectRepository.UpdateAsync(subject, cancellationToken);

        return Unit.Value;
    }
}
