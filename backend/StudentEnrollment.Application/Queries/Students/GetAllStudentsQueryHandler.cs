using MediatR;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Students;

public sealed class GetAllStudentsQueryHandler(
    IStudentRepository studentRepository)
    : IRequestHandler<GetAllStudentsQuery, PagedStudentsResponse>
{
    public async Task<PagedStudentsResponse> Handle(
        GetAllStudentsQuery request,
        CancellationToken cancellationToken)
    {
        var students = await studentRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            cancellationToken
        );

        var totalCount = await studentRepository.CountAsync(cancellationToken);

        var studentDtos = students.Select(s => new StudentDto(
            Id: s.Id,
            StudentCode: s.StudentCode,
            UserId: s.UserId,
            Name: s.Name,
            Email: s.User.Email,
            IsActive: s.User.IsActive,
            CreatedAt: s.CreatedAt,
            UpdatedAt: s.CreatedAt
        )).ToList();

        return new PagedStudentsResponse(
            Students: studentDtos,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling(totalCount / (double)request.PageSize)
        );
    }
}
