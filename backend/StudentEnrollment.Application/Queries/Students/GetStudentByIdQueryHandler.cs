using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Students;

public sealed class GetStudentByIdQueryHandler(
    IStudentRepository studentRepository) 
    : IRequestHandler<GetStudentByIdQuery, StudentDetailDto>
{
    public async Task<StudentDetailDto> Handle(
        GetStudentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException($"Estudiante con ID {request.StudentId} no encontrado", "STUDENT_NOT_FOUND");

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
