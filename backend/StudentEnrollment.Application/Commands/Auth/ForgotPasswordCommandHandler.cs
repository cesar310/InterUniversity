using MediatR;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService
) : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    public async Task<Result<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Buscar usuario por email
        var user = await userRepository.GetByEmailAsync(request.Email.ToLower(), cancellationToken);

        if (user is null)
        {
            // Siempre responder success para evitar enumeración de emails
            return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse(
                Success: true,
                Message: "Si el email existe, recibirás una nueva contraseña temporal en breve.",
                EmailSent: false
            ));
        }

        // Generar nueva contraseña temporal
        var temporaryPassword = passwordHasher.GenerateTemporaryPassword();
        var newPasswordHash = passwordHasher.HashPassword(temporaryPassword);

        // Actualizar hash y requerir cambio
        user.PasswordHash = newPasswordHash;
        user.RequirePasswordChange();
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        // Enviar email
        var emailSent = false;
        try
        {
            await emailService.SendTemporaryPasswordEmailAsync(
                user.Email,
                user.Student?.Name ?? "Usuario",
                temporaryPassword,
                cancellationToken
            );
            emailSent = true;
        }
        catch
        {
            // Email falló
        }

        return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse(
            Success: true,
            Message: emailSent
                ? "Nueva contraseña temporal enviada a tu email."
                : "Si el email existe, recibirás una nueva contraseña temporal en breve.",
            EmailSent: emailSent
        ));
    }
}