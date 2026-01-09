using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Application.Queries.Enrollments;

public sealed record GetAllEnrollmentsQuery(
    int Page = 1,
    int PageSize = 10,
    int? StudentId = null,
    int? SubjectId = null,
    EnrollmentStatus? Status = null
) : IRequest<PagedEnrollmentsResponse>;
