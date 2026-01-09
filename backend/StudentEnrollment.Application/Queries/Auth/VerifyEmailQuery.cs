using MediatR;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Queries.Auth;

public sealed record VerifyEmailQuery(
    string Token
) : IRequest<Result<VerifyEmailResponse>>;

public sealed record VerifyEmailResponse(
    bool Success,
    string Message,
    string? RedirectUrl
);