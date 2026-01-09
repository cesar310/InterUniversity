using MediatR;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Queries.Auth;

public sealed class VerifyEmailQueryHandler(
    IUserRepository userRepository
) : IRequestHandler<VerifyEmailQuery, Result<VerifyEmailResponse>>
{
    public async Task<Result<VerifyEmailResponse>> Handle(
        VerifyEmailQuery request,
        CancellationToken cancellationToken)
    {
        // Buscar usuario con el token
        var user = await userRepository.GetByEmailVerificationTokenAsync(request.Token, cancellationToken);

        if (user is null)
        {
            return Result<VerifyEmailResponse>.Success(new VerifyEmailResponse(
                Success: false,
                Message: "Token de verificación inválido o expirado.",
                RedirectUrl: "/login?error=invalid_token"
            ));
        }

        // Verificar si ya está verificado
        if (user.EmailVerified)
        {
            return Result<VerifyEmailResponse>.Success(new VerifyEmailResponse(
                Success: true,
                Message: "El email ya está verificado.",
                RedirectUrl: "/login?message=email_already_verified"
            ));
        }

        // Verificar email con token
        if (!user.VerifyEmail(request.Token))
        {
            return Result<VerifyEmailResponse>.Success(new VerifyEmailResponse(
                Success: false,
                Message: "Token de verificación inválido o expirado.",
                RedirectUrl: "/login?error=invalid_token"
            ));
        }

        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result<VerifyEmailResponse>.Success(new VerifyEmailResponse(
            Success: true,
            Message: "Email verificado exitosamente.",
            RedirectUrl: "/login?message=email_verified"
        ));
    }
}