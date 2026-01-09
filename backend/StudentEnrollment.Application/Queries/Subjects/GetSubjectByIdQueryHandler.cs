using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Subjects;

public sealed class GetSubjectByIdQueryHandler(
    ISubjectRepository subjectRepository)
    : IRequestHandler<GetSubjectByIdQuery, SubjectDetailDto>
{
    public async Task<SubjectDetailDto> Handle(
        GetSubjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        return new SubjectDetailDto(
            Id: subject.Id,
            Name: subject.Name,
            Description: subject.Description,
            Credits: subject.Credits,
            ProfessorId: subject.ProfessorId,
            ProfessorName: subject.Professor.Name,
            ProfessorSpecialization: subject.Professor.Specialization,
            ProfessorEmail: subject.Professor.Email,
            EnrolledStudents: subject.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active),
            IsActive: subject.IsActive,
            CreatedAt: subject.CreatedAt
        );
    }
}
