using MediatR;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed record ForgotPasswordCommand(
    string Email
) : IRequest<Result<ForgotPasswordResponse>>;

public sealed record ForgotPasswordResponse(
    bool Success,
    string Message,
    bool EmailSent
);