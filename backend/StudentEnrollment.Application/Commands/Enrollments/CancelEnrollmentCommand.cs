using MediatR;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed record CancelEnrollmentCommand(
    int StudentId,
    int SubjectId
) : IRequest<Unit>;
