using MediatR;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed record CreateProfessorCommand(
    string Name,
    string? Specialization,
    string? Email,
    string? Phone
) : IRequest<int>;
