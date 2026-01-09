using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Commands.Auth;

/// <summary>
/// Command para login de usuario
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;
