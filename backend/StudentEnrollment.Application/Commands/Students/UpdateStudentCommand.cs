using MediatR;

namespace StudentEnrollment.Application.Commands.Students;

public sealed record UpdateStudentCommand(
    int StudentId,
    string Name,
    string StudentCode
) : IRequest<Unit>;
