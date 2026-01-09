using MediatR;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class CreateProfessorCommandHandler(
    IProfessorRepository professorRepository)
    : IRequestHandler<CreateProfessorCommand, int>
{
    public async Task<int> Handle(
        CreateProfessorCommand request,
        CancellationToken cancellationToken)
    {
        var professor = new Professor
        {
            Name = request.Name,
            Specialization = request.Specialization,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await professorRepository.CreateAsync(professor, cancellationToken);

        return created.Id;
    }
}
