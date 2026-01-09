using MediatR;

namespace StudentEnrollment.Application.Commands.Students;

public sealed record DeleteStudentCommand(int StudentId) : IRequest<Unit>;
