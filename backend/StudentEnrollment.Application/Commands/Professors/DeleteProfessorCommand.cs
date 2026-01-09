using MediatR;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed record DeleteProfessorCommand(int ProfessorId) : IRequest<Unit>;
