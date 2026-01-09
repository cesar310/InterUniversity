using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class DeactivateProfessorCommandHandler(
    IProfessorRepository professorRepository,
    ISubjectRepository subjectRepository)
    : IRequestHandler<DeactivateProfessorCommand, Unit>
{
    public async Task<Unit> Handle(
        DeactivateProfessorCommand request,
        CancellationToken cancellationToken)
    {
        var professor = await professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new NotFoundException($"Profesor con ID {request.ProfessorId} no encontrado", "PROFESSOR_NOT_FOUND");

        // Validar que no tenga materias activas
        var activeSubjectsCount = await subjectRepository.CountByProfessorIdAsync(request.ProfessorId, cancellationToken);

        if (activeSubjectsCount > 0)
        {
            throw new BusinessRuleException(
                $"No se puede desactivar un profesor con {activeSubjectsCount} materias activas",
                "PROFESSOR_HAS_ACTIVE_SUBJECTS"
            );
        }

        // Desactivar el profesor
        professor.IsActive = false;
        professor.UpdatedAt = DateTime.UtcNow;

        // Guardar cambios
        await professorRepository.UpdateAsync(professor, cancellationToken);

        return Unit.Value;
    }
}
