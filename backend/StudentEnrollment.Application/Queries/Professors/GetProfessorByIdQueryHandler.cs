using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Professors;

public sealed class GetProfessorByIdQueryHandler(
    IProfessorRepository professorRepository,
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<GetProfessorByIdQuery, ProfessorDetailDto>
{
    public async Task<ProfessorDetailDto> Handle(
        GetProfessorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var professor = await professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new NotFoundException($"Profesor con ID {request.ProfessorId} no encontrado", "PROFESSOR_NOT_FOUND");

        var maxSubjectsPerProfessor = await systemConfigRepository.GetIntValueAsync("max_subjects_per_professor", cancellationToken) ?? 3;

        return new ProfessorDetailDto(
            Id: professor.Id,
            Name: professor.Name,
            Specialization: professor.Specialization,
            Email: professor.Email,
            Phone: professor.Phone,
            IsActive: professor.IsActive,
            TotalSubjects: professor.Subjects.Count(s => s.IsActive),
            MaxAllowed: maxSubjectsPerProfessor,
            CreatedAt: professor.CreatedAt,
            UpdatedAt: professor.UpdatedAt
        );
    }
}
