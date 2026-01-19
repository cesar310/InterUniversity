using MediatR;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Enums;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed class EnrollStudentCommandHandler(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ISubjectRepository subjectRepository,
    ISystemConfigRepository systemConfigRepository,
    IEmailService emailService)
    : IRequestHandler<EnrollStudentCommand, EnrollmentDto>
{
    public async Task<EnrollmentDto> Handle(
        EnrollStudentCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que las inscripciones estén abiertas
        var enrollmentOpen = await systemConfigRepository.GetBoolValueAsync("enrollment_open", cancellationToken) ?? true;
        if (!enrollmentOpen)
        {
            throw new BusinessRuleException(
                "Las inscripciones están cerradas en este momento",
                "ENROLLMENT_CLOSED"
            );
        }

        // Verificar que el estudiante existe
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        // Verificar que la materia existe y está activa
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        if (!subject.IsActive)
        {
            throw new BusinessRuleException("No se puede inscribir en una materia inactiva", "SUBJECT_INACTIVE");
        }

        // Verificar que no esté ya inscrito
        if (await enrollmentRepository.HasActiveEnrollmentAsync(request.StudentId, request.SubjectId, cancellationToken))
        {
            throw new DuplicateException("Enrollment", "StudentId-SubjectId", $"{request.StudentId}-{request.SubjectId}");
        }

        // Validar máximo de materias permitidas
        var maxSubjectsPerStudent = await systemConfigRepository.GetIntValueAsync("max_subjects_per_student", cancellationToken) ?? 3;
        var currentEnrollmentsCount = await enrollmentRepository.CountEnrollmentsByStudentAsync(request.StudentId, cancellationToken);

        if (currentEnrollmentsCount >= maxSubjectsPerStudent)
        {
            throw new BusinessRuleException(
                $"El estudiante ya tiene el máximo de {maxSubjectsPerStudent} inscripciones permitidas",
                "MAX_ENROLLMENTS_EXCEEDED"
            );
        }

        // Validar si se permite inscribir materias con el mismo profesor
        var allowSameProfessor = await systemConfigRepository.GetBoolValueAsync("allow_same_professor", cancellationToken) ?? false;
        
        if (!allowSameProfessor)
        {
            // Solo validar si la configuración no permite el mismo profesor
            var studentEnrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
            var hasSameProfessor = studentEnrollments.Any(e => 
                e.Status == EnrollmentStatus.Active && 
                e.Subject.ProfessorId == subject.ProfessorId);

            if (hasSameProfessor)
            {
                throw new BusinessRuleException(
                    $"El sistema no permite inscribir múltiples materias con el mismo profesor ({subject.Professor.Name})",
                    "DUPLICATE_PROFESSOR_ENROLLMENT"
                );
            }
        }

        // Crear la inscripción
        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            SubjectId = request.SubjectId,
            Status = EnrollmentStatus.Active,
            EnrolledAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await enrollmentRepository.CreateAsync(enrollment, cancellationToken);

        // Enviar email de confirmación
        await emailService.SendEnrollmentConfirmationAsync(
            student.User.Email,
            student.Name,
            subject.Name,
            cancellationToken
        );

        return new EnrollmentDto(
            StudentId: request.StudentId,
            SubjectId: request.SubjectId,
            StudentCode: student.StudentCode,
            StudentEmail: student.User.Email,
            SubjectName: subject.Name,
            ProfessorName: subject.Professor.Name,
            Status: EnrollmentStatus.Active,
            EnrolledAt: enrollment.EnrolledAt,
            UpdatedAt: enrollment.UpdatedAt
        );
    }
}
