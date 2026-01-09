using MediatR;
using StudentEnrollment.Domain.Enums;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed class CancelEnrollmentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ISubjectRepository subjectRepository)
    : IRequestHandler<CancelEnrollmentCommand, Unit>
{
    public async Task<Unit> Handle(
        CancelEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que el estudiante existe
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        // Verificar que la materia existe
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        // Verificar que existe la inscripción activa
        if (!await enrollmentRepository.HasActiveEnrollmentAsync(request.StudentId, request.SubjectId, cancellationToken))
        {
            throw new NotFoundException("Inscripción activa no encontrada", "ENROLLMENT_NOT_FOUND");
        }

        // Obtener la inscripción
        var enrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        var enrollment = enrollments.FirstOrDefault(e => 
            e.SubjectId == request.SubjectId && 
            e.Status == EnrollmentStatus.Active);

        if (enrollment == null)
        {
            throw new NotFoundException("Inscripción activa no encontrada", "ENROLLMENT_NOT_FOUND");
        }

        // Cambiar estado a cancelado
        enrollment.Status = EnrollmentStatus.Cancelled;
        enrollment.UpdatedAt = DateTime.UtcNow;

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return Unit.Value;
    }
}
