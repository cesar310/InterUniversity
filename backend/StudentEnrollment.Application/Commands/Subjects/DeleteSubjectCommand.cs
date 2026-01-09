using MediatR;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed record DeleteSubjectCommand(int SubjectId) : IRequest<Unit>;
