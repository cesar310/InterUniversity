using MediatR;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed record DeactivateProfessorCommand(int ProfessorId) : IRequest<Unit>;
