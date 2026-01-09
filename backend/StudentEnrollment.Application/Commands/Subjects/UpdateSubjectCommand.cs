using MediatR;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed record UpdateSubjectCommand(
    int SubjectId,
    string Name,
    string? Description,
    int Credits,
    int ProfessorId,
    bool IsActive
) : IRequest<Unit>;
