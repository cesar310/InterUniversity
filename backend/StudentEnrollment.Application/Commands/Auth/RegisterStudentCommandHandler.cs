using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Common;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Auth;

public sealed class RegisterStudentCommandHandler(
    IUserRepository userRepository,
    IStudentRepository studentRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService
) : IRequestHandler<RegisterStudentCommand, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validar que el email no exista
        if (await userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result<StudentDto>.Failure(
                "El email ya está registrado",
                "EMAIL_ALREADY_EXISTS"
            );
        }

        // 2. Generar contraseña temporal
        var temporaryPassword = passwordHasher.GenerateTemporaryPassword();
        var passwordHash = passwordHasher.HashPassword(temporaryPassword);

        // 3. Obtener rol de estudiante
        var studentRole = await roleRepository.GetByNameAsync("student", cancellationToken);
        if (studentRole is null)
        {
            return Result<StudentDto>.Failure(
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

        // Marcar email como verificado (usuario creado por administrador)
        user.MarkEmailAsVerified();

        // Requerir cambio de contraseña temporal
        user.RequirePasswordChange();

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

        // 7. Enviar email con contraseña temporal
        await emailService.SendTemporaryPasswordEmailAsync(
            request.Email,
            request.Name,
            temporaryPassword,
            cancellationToken
        );

        // 9. Retornar DTO
        var studentDto = new StudentDto(
            Id: student.Id,
            StudentCode: student.StudentCode,
            UserId: user.Id,
            Name: student.Name,
            Email: user.Email,
            IsActive: user.IsActive,
            CreatedAt: student.CreatedAt,
            UpdatedAt: user.UpdatedAt
        );

        return Result<StudentDto>.Success(studentDto);
    }
}
