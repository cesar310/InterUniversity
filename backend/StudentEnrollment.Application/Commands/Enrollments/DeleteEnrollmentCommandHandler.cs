using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed class DeleteEnrollmentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ISubjectRepository subjectRepository)
    : IRequestHandler<DeleteEnrollmentCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que el estudiante existe
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        // Verificar que la materia existe
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        // Verificar que existe la inscripción
        var enrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        var enrollment = enrollments.FirstOrDefault(e => e.SubjectId == request.SubjectId);

        if (enrollment == null)
        {
            throw new NotFoundException("Inscripción no encontrada", "ENROLLMENT_NOT_FOUND");
        }

        // Eliminar la inscripción
        await enrollmentRepository.DeleteAsync(request.StudentId, request.SubjectId, cancellationToken);

        return Unit.Value;
    }
}
