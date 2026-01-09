using MediatR;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Enrollments;

public sealed class GetAllEnrollmentsQueryHandler(
    IEnrollmentRepository enrollmentRepository)
    : IRequestHandler<GetAllEnrollmentsQuery, PagedEnrollmentsResponse>
{
    public async Task<PagedEnrollmentsResponse> Handle(
        GetAllEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        var enrollments = await enrollmentRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            cancellationToken
        );

        var totalCount = await enrollmentRepository.CountAsync(cancellationToken);

        // Aplicar filtros en memoria (idealmente esto debería estar en el repositorio)
        if (request.StudentId.HasValue)
        {
            enrollments = enrollments.Where(e => e.StudentId == request.StudentId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            enrollments = enrollments.Where(e => e.SubjectId == request.SubjectId.Value);
        }

        if (request.Status.HasValue)
        {
            enrollments = enrollments.Where(e => e.Status == request.Status.Value);
        }

        var enrollmentDtos = enrollments.Select(e => new EnrollmentDto(
            StudentId: e.StudentId,
            SubjectId: e.SubjectId,
            StudentCode: e.Student.StudentCode,
            StudentEmail: e.Student.User.Email,
            SubjectName: e.Subject.Name,
            ProfessorName: e.Subject.Professor.Name,
            Status: e.Status,
            EnrolledAt: e.EnrolledAt,
            UpdatedAt: e.UpdatedAt
        )).ToList();

        return new PagedEnrollmentsResponse(
            Enrollments: enrollmentDtos,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling(totalCount / (double)request.PageSize)
        );
    }
}
