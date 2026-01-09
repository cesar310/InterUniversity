using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Subjects;

public sealed record GetAllSubjectsQuery(
    int Page = 1,
    int PageSize = 10,
    bool? IsActive = true,
    int? ProfessorId = null
) : IRequest<PagedSubjectsResponse>;
