using MediatR;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Subjects;

public sealed class GetAllSubjectsQueryHandler(
    ISubjectRepository subjectRepository)
    : IRequestHandler<GetAllSubjectsQuery, PagedSubjectsResponse>
{
    public async Task<PagedSubjectsResponse> Handle(
        GetAllSubjectsQuery request,
        CancellationToken cancellationToken)
    {
        var subjects = await subjectRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            cancellationToken
        );

        var totalCount = await subjectRepository.CountAsync(cancellationToken);

        var subjectDtos = subjects.Select(s => new SubjectDto(
            Id: s.Id,
            Name: s.Name,
            Description: s.Description,
            Credits: s.Credits,
            ProfessorId: s.ProfessorId,
            ProfessorName: s.Professor.Name,
            EnrolledStudents: s.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active),
            IsActive: s.IsActive,
            CreatedAt: s.CreatedAt
        )).ToList();

        return new PagedSubjectsResponse(
            Subjects: subjectDtos,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling(totalCount / (double)request.PageSize)
        );
    }
}
