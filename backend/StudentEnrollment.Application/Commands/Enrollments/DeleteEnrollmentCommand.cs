using MediatR;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed record DeleteEnrollmentCommand(
    int StudentId,
    int SubjectId
) : IRequest<Unit>;
