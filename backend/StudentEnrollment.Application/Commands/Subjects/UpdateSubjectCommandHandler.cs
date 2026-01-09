using MediatR;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class UpdateSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IProfessorRepository professorRepository,
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<UpdateSubjectCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateSubjectCommand request,
        CancellationToken cancellationToken)
    {
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException($"Materia con ID {request.SubjectId} no encontrada", "SUBJECT_NOT_FOUND");

        // Actualizar nombre si cambió
        if (request.Name != subject.Name)
        {
            if (await subjectRepository.SubjectNameExistsAsync(request.Name, cancellationToken))
            {
                throw new DuplicateException("Subject", "Name", request.Name);
            }
            subject.Name = request.Name;
        }

        // Actualizar créditos
        subject.Credits = request.Credits;
        
        // Actualizar descripción
        subject.Description = request.Description;

        // Actualizar profesor si cambió
        if (request.ProfessorId != subject.ProfessorId)
        {
            var professor = await professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
                ?? throw new NotFoundException($"Profesor con ID {request.ProfessorId} no encontrado", "PROFESSOR_NOT_FOUND");

            if (!professor.IsActive)
            {
                throw new BusinessRuleException("No se puede asignar un profesor inactivo a una materia", "PROFESSOR_INACTIVE");
            }

            // Validar que el nuevo profesor no exceda el máximo de materias
            var maxSubjectsPerProfessor = await systemConfigRepository.GetIntValueAsync("max_subjects_per_professor", cancellationToken) ?? 3;
            var currentSubjectsCount = await subjectRepository.CountByProfessorIdAsync(request.ProfessorId, cancellationToken);

            if (currentSubjectsCount >= maxSubjectsPerProfessor)
            {
                throw new BusinessRuleException(
                    $"El profesor ya tiene el máximo de {maxSubjectsPerProfessor} materias permitidas",
                    "MAX_SUBJECTS_EXCEEDED"
                );
            }

            subject.ProfessorId = request.ProfessorId;
        }

        // Actualizar estado
        subject.IsActive = request.IsActive;

        await subjectRepository.UpdateAsync(subject, cancellationToken);

        return Unit.Value;
    }
}
