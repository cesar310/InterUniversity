using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class UpdateProfessorCommandHandler(
    IProfessorRepository professorRepository)
    : IRequestHandler<UpdateProfessorCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateProfessorCommand request,
        CancellationToken cancellationToken)
    {
        var professor = await professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new NotFoundException($"Profesor con ID {request.ProfessorId} no encontrado", "PROFESSOR_NOT_FOUND");

        professor.Name = request.Name;
        professor.Specialization = request.Specialization;
        professor.Email = request.Email;
        professor.Phone = request.Phone;
        professor.IsActive = request.IsActive;
        professor.UpdatedAt = DateTime.UtcNow;

        await professorRepository.UpdateAsync(professor, cancellationToken);

        return Unit.Value;
    }
}
