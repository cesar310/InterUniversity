using MediatR;
using StudentEnrollment.Domain.Enums;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Enrollments;

public sealed class GetMyEnrollmentsQueryHandler(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<GetMyEnrollmentsQuery, MyEnrollmentsResponse>
{
    public async Task<MyEnrollmentsResponse> Handle(
        GetMyEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

        var enrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

        // Filtrar por estado si se especifica
        if (request.Status.HasValue)
        {
            enrollments = enrollments.Where(e => e.Status == request.Status.Value);
        }

        var maxSubjectsPerStudent = await systemConfigRepository.GetIntValueAsync("max_subjects_per_student", cancellationToken) ?? 3;

        var enrollmentDtos = enrollments.Select(e => new MyEnrollmentDto(
            SubjectId: e.SubjectId,
            SubjectName: e.Subject.Name,
            Credits: e.Subject.Credits,
            ProfessorName: e.Subject.Professor.Name,
            Status: e.Status,
            EnrolledAt: e.EnrolledAt
        )).ToList();

        var totalCredits = enrollmentDtos
            .Where(e => e.Status == EnrollmentStatus.Active)
            .Sum(e => e.Credits);

        var activeEnrollments = enrollmentDtos
            .Count(e => e.Status == EnrollmentStatus.Active);

        return new MyEnrollmentsResponse(
            StudentId: request.StudentId,
            StudentName: student.Name,
            Enrollments: enrollmentDtos,
            TotalCredits: totalCredits,
            ActiveEnrollments: activeEnrollments,
            MaxAllowed: maxSubjectsPerStudent
        );
    }
}
