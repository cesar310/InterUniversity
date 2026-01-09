using MediatR;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed class SelfRegisterStudentCommandHandler(
    IUserRepository userRepository,
    IStudentRepository studentRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService
) : IRequestHandler<SelfRegisterStudentCommand, Result<SelfRegisterResponse>>
{
    public async Task<Result<SelfRegisterResponse>> Handle(
        SelfRegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validar que el email no exista
        if (await userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result<SelfRegisterResponse>.Failure(
                "El email ya está registrado",
                "EMAIL_ALREADY_EXISTS"
            );
        }

        // 2. Generar hash de la contraseña proporcionada
        var passwordHash = passwordHasher.HashPassword(request.Password);

        // 3. Obtener rol de estudiante
        var studentRole = await roleRepository.GetByNameAsync("student", cancellationToken);
        if (studentRole is null)
        {
            return Result<SelfRegisterResponse>.Failure(
                "Error de configuración: rol de estudiante no encontrado",
                "ROLE_NOT_FOUND"
            );
        }

        // 4. Crear usuario
        var user = new User
        {
            Email = request.Email.ToLower(),
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = new List<Role> { studentRole }
        };

        // Marcar que debe cambiar contraseña (opcional, ya que proporcionó una)
        // user.RequirePasswordChange(); // Comentado, ya que el usuario proporcionó la contraseña
        user.RequireEmailVerification();

        await userRepository.CreateAsync(user, cancellationToken);

        // 5. Crear estudiante (código generado automáticamente por DB trigger)
        var student = new Student
        {
            StudentCode = string.Empty, // Será generado por el trigger de la DB
            UserId = user.Id,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        await studentRepository.CreateAsync(student, cancellationToken);

        // 7. Enviar email de verificación (no contraseña temporal)
        var emailSent = false;
        try
        {
            await emailService.SendEmailVerificationAsync(
                user.Email,
                request.Name,
                user.EmailVerificationToken!,
                cancellationToken
            );
            emailSent = true;
        }
        catch
        {
            // Email falló, pero usuario creado
        }

        // 8. Retornar response
        var response = new SelfRegisterResponse(
            UserId: user.Id,
            StudentId: student.Id,
            Email: user.Email,
            Name: student.Name,
            StudentCode: student.StudentCode,
            EmailSent: emailSent,
            Message: emailSent
                ? "Registro exitoso. Revisa tu email para verificar tu cuenta."
                : "Registro exitoso, pero hubo un problema enviando el email de verificación. Contacta al administrador."
        );

        return Result<SelfRegisterResponse>.Success(response);
    }
}