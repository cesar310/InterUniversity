using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator
) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Buscar usuario por email
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<LoginResponse>.Failure(
                "Email o contraseña incorrectos",
                "AUTHENTICATION_FAILED"
            );
        }

        // 2. Verificar contraseña
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure(
                "Email o contraseña incorrectos",
                "AUTHENTICATION_FAILED"
            );
        }

        // 3. Verificar email verificado
        if (!user.EmailVerified)
        {
            return Result<LoginResponse>.Failure(
                "Debes verificar tu email antes de iniciar sesión",
                "EMAIL_NOT_VERIFIED"
            );
        }

        // 4. Verificar cuenta activa
        if (!user.IsActive)
        {
            return Result<LoginResponse>.Failure(
                "Tu cuenta ha sido desactivada. Contacta al administrador",
                "ACCOUNT_INACTIVE"
            );
        }

        // 5. Generar token JWT
        var roles = user.Roles.Select(r => r.Name).ToList();
        var token = jwtTokenGenerator.GenerateToken(user, roles);

        // 6. Construir response
        var response = new LoginResponse(
            Token: token,
            TokenType: "Bearer",
            ExpiresIn: 3600,
            User: new UserInfoDto(
                Id: user.Id,
                Email: user.Email,
                Roles: roles,
                StudentId: user.Student?.Id,
                MustChangePassword: user.MustChangePassword,
                EmailVerified: user.EmailVerified
            )
        );

        return Result<LoginResponse>.Success(response);
    }
}
