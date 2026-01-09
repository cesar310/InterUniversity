using MediatR;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed record UpdateProfessorCommand(
    int ProfessorId,
    string Name,
    string Specialization,
    string Email,
    string Phone,
    bool IsActive
) : IRequest<Unit>;
