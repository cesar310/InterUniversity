using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Queries.Enrollments;

public sealed class GetClassmatesBySubjectQueryHandler(
    IEnrollmentRepository enrollmentRepository)
    : IRequestHandler<GetClassmatesBySubjectQuery, IEnumerable<ClassmateDto>>
{
    public async Task<IEnumerable<ClassmateDto>> Handle(
        GetClassmatesBySubjectQuery request,
        CancellationToken cancellationToken)
    {
        var classmates = await enrollmentRepository.GetClassmatesBySubjectAsync(request.SubjectId, cancellationToken);
        
        return classmates.Select(c => new ClassmateDto(
            SubjectName: c.SubjectName,
            StudentName: c.StudentName
        )).ToList();
    }
}
