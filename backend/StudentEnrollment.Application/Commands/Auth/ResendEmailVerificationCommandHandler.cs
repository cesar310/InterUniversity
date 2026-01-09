using MediatR;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed class ResendEmailVerificationCommandHandler(
    IUserRepository userRepository,
    IEmailService emailService
) : IRequestHandler<ResendEmailVerificationCommand, Result<ResendEmailVerificationResponse>>
{
    public async Task<Result<ResendEmailVerificationResponse>> Handle(
        ResendEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        // Buscar usuario
        var user = await userRepository.GetByEmailAsync(request.Email.ToLower(), cancellationToken);

        if (user is null)
        {
            return Result<ResendEmailVerificationResponse>.Failure(
                "Usuario no encontrado",
                "USER_NOT_FOUND"
            );
        }

        // Verificar que no esté ya verificado
        if (user.EmailVerified)
        {
            return Result<ResendEmailVerificationResponse>.Failure(
                "El email ya está verificado",
                "EMAIL_ALREADY_VERIFIED"
            );
        }

        // Generar nuevo token
        user.GenerateEmailVerificationToken();
        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        // Enviar email
        var emailSent = false;
        try
        {
            await emailService.SendEmailVerificationAsync(
                user.Email,
                user.Student?.Name ?? "Usuario",
                user.EmailVerificationToken!,
                cancellationToken
            );
            emailSent = true;
        }
        catch
        {
            // Email falló
        }

        var details = emailSent ? new ResendEmailVerificationDetails(
            Email: user.Email,
            ExpiresIn: "24 horas",
            AttemptsRemaining: 2 // Podría implementar rate limiting real
        ) : null;

        return Result<ResendEmailVerificationResponse>.Success(new ResendEmailVerificationResponse(
            Success: emailSent,
            Message: emailSent
                ? "Email de verificación reenviado exitosamente."
                : "Error enviando el email de verificación.",
            Details: details
        ));
    }
}