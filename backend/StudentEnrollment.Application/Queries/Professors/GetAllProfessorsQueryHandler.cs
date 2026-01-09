using MediatR;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Professors;

public sealed class GetAllProfessorsQueryHandler(
    IProfessorRepository professorRepository)
    : IRequestHandler<GetAllProfessorsQuery, PagedProfessorsResponse>
{
    public async Task<PagedProfessorsResponse> Handle(
        GetAllProfessorsQuery request,
        CancellationToken cancellationToken)
    {
        // Usar la vista view_professors que ya tiene los contadores calculados
        var professorsWithWorkload = await professorRepository.GetProfessorsWithWorkloadAsync(
            request.Page,
            request.PageSize,
            cancellationToken
        );

        var totalCount = await professorRepository.CountAsync(cancellationToken);

        var professorDtos = professorsWithWorkload.Select(p => new ProfessorDto(
            Id: p.Id,
            Name: p.Name,
            Specialization: p.Specialization,
            Email: p.Email,
            Phone: p.Phone,
            IsActive: p.IsActive,
            TotalSubjects: p.TotalSubjects,
            MaxAllowed: p.MaxAllowed,
            CreatedAt: DateTime.Now, // La vista no tiene CreatedAt, usar fecha actual
            UpdatedAt: DateTime.Now  // La vista no tiene UpdatedAt, usar fecha actual
        )).ToList();

        return new PagedProfessorsResponse(
            Professors: professorDtos,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: (int)Math.Ceiling(totalCount / (double)request.PageSize)
        );
    }
}
