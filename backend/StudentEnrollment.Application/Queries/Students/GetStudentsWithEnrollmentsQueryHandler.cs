using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Queries.Students;

public sealed class GetStudentsWithEnrollmentsQueryHandler(
    IStudentRepository studentRepository)
    : IRequestHandler<GetStudentsWithEnrollmentsQuery, IEnumerable<StudentWithEnrollmentsDto>>
{
    public async Task<IEnumerable<StudentWithEnrollmentsDto>> Handle(
        GetStudentsWithEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        var studentsWithEnrollments = await studentRepository.GetStudentsWithEnrollmentsAsync(cancellationToken);
        
        return studentsWithEnrollments.Select(s => new StudentWithEnrollmentsDto(
            StudentId: s.StudentId,
            StudentName: s.StudentName,
            StudentCode: s.StudentCode,
            Email: s.Email,
            IsActive: s.IsActive,
            EnrolledSubjects: s.EnrolledSubjects,
            MaxAllowed: s.MaxAllowed,
            Subjects: s.Subjects
        )).ToList();
    }
}
