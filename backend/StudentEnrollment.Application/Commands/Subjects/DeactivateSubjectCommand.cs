using MediatR;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed record DeactivateSubjectCommand(int SubjectId) : IRequest<Unit>;
