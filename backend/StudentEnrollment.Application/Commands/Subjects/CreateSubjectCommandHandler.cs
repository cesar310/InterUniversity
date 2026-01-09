using MediatR;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Exceptions;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Commands.Subjects;

public sealed class CreateSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IProfessorRepository professorRepository,
    ISystemConfigRepository systemConfigRepository)
    : IRequestHandler<CreateSubjectCommand, int>
{
    public async Task<int> Handle(
        CreateSubjectCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que el nombre no esté duplicado
        if (await subjectRepository.SubjectNameExistsAsync(request.Name, cancellationToken))
        {
            throw new DuplicateException("Subject", "Name", request.Name);
        }

        // Verificar que el profesor exista y esté activo
        var professor = await professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new NotFoundException($"Profesor con ID {request.ProfessorId} no encontrado", "PROFESSOR_NOT_FOUND");

        if (!professor.IsActive)
        {
            throw new BusinessRuleException("No se puede asignar un profesor inactivo a una materia", "PROFESSOR_INACTIVE");
        }

        // Validar que el profesor no exceda el máximo de materias permitidas
        var maxSubjectsPerProfessor = await systemConfigRepository.GetIntValueAsync("max_subjects_per_professor", cancellationToken) ?? 3;
        var currentSubjectsCount = await subjectRepository.CountByProfessorIdAsync(request.ProfessorId, cancellationToken);

        if (currentSubjectsCount >= maxSubjectsPerProfessor)
        {
            throw new BusinessRuleException(
                $"El profesor ya tiene el máximo de {maxSubjectsPerProfessor} materias permitidas",
                "MAX_SUBJECTS_EXCEEDED"
            );
        }

        // Crear la materia
        var subject = new Subject
        {
            Name = request.Name,
            Description = request.Description,
            Credits = request.Credits,
            ProfessorId = request.ProfessorId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await subjectRepository.CreateAsync(subject, cancellationToken);

        return created.Id;
    }
}
