using MediatR;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed record SelfRegisterStudentCommand(
    string Email,
    string Name,
    string Password
) : IRequest<Result<SelfRegisterResponse>>;

public sealed record SelfRegisterResponse(
    int UserId,
    int StudentId,
    string Email,
    string Name,
    string StudentCode,
    bool EmailSent,
    string Message
);