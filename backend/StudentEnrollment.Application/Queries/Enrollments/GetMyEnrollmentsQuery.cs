using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Application.Queries.Enrollments;

public sealed record GetMyEnrollmentsQuery(
    int StudentId,
    EnrollmentStatus? Status = null
) : IRequest<MyEnrollmentsResponse>;
