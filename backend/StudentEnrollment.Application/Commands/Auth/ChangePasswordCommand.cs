using MediatR;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Commands.Auth;

/// <summary>
/// Command para cambiar contraseña
/// </summary>
public sealed record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result>;
