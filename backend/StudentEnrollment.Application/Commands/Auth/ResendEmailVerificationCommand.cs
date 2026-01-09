using MediatR;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed record ResendEmailVerificationCommand(
    string Email
) : IRequest<Result<ResendEmailVerificationResponse>>;

public sealed record ResendEmailVerificationResponse(
    bool Success,
    string Message,
    ResendEmailVerificationDetails? Details
);

public sealed record ResendEmailVerificationDetails(
    string Email,
    string ExpiresIn,
    int AttemptsRemaining
);