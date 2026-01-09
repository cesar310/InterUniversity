using MediatR;

namespace StudentEnrollment.Application.Commands.Students;

public sealed record DeactivateStudentCommand(int StudentId) : IRequest<Unit>;
