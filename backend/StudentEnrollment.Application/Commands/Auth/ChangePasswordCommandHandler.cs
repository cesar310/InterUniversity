using MediatR;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher
) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Obtener usuario
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Usuario no encontrado", "USER_NOT_FOUND");
        }

        // 2. Verificar contraseña actual
        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure(
                "La contraseña actual es incorrecta",
                "INVALID_CURRENT_PASSWORD"
            );
        }

        // 3. Verificar que la nueva contraseña no sea igual a la actual
        if (passwordHasher.VerifyPassword(request.NewPassword, user.PasswordHash))
        {
            return Result.Failure(
                "La nueva contraseña debe ser diferente a la actual",
                "SAME_PASSWORD"
            );
        }

        // 4. Actualizar contraseña
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.MarkPasswordChanged();
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
