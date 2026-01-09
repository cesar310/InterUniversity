using MediatR;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed record CreateSubjectCommand(
    string Name,
    string? Description,
    int Credits,
    int ProfessorId,
    bool IsActive = true
) : IRequest<int>;
