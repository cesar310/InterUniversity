using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Professors;

public sealed class DeleteProfessorCommandHandler(
    IProfessorRepository professorRepository,
    ISubjectRepository subjectRepository)
    : IRequestHandler<DeleteProfessorCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteProfessorCommand request,
        CancellationToken cancellationToken)
    {
        var professor = await professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new NotFoundException($"Profesor con ID {request.ProfessorId} no encontrado", "PROFESSOR_NOT_FOUND");

        // Validar que no tenga materias activas
        var activeSubjectsCount = await subjectRepository.CountByProfessorIdAsync(request.ProfessorId, cancellationToken);

        if (activeSubjectsCount > 0)
        {
            throw new BusinessRuleException(
                $"No se puede eliminar un profesor con {activeSubjectsCount} materias activas",
                "PROFESSOR_HAS_ACTIVE_SUBJECTS"
            );
        }

        // Eliminar el profesor
        await professorRepository.DeleteAsync(request.ProfessorId, cancellationToken);

        return Unit.Value;
    }
}
