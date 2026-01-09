using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Students;

public sealed class GetMyProfileQueryHandler(
    IStudentRepository studentRepository)
    : IRequestHandler<GetMyProfileQuery, StudentDetailDto>
{
    public async Task<StudentDetailDto> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Perfil de estudiante no encontrado", "STUDENT_NOT_FOUND");

        return new StudentDetailDto(
            Id: student.Id,
            StudentCode: student.StudentCode,
            UserId: student.UserId,
            Email: student.User.Email,
            IsActive: student.User.IsActive,
            EmailVerified: student.User.EmailVerified,
            MustChangePassword: student.User.MustChangePassword,
            CreatedAt: student.CreatedAt,
            UpdatedAt: student.CreatedAt
        );
    }
}
