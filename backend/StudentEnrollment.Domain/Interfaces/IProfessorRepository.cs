using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Domain.Interfaces;

public interface IProfessorRepository
{
    Task<Professor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Professor?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Professor>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Professor> CreateAsync(Professor professor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Professor professor, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken = default);
    Task<bool> EmployeeNumberExistsExcludingProfessorAsync(string employeeNumber, int professorId, CancellationToken cancellationToken = default);
    Task<int> GetSubjectCountAsync(int professorId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene profesores desde la vista view_professors con contadores de materias
    /// </summary>
    Task<IEnumerable<ProfessorWithWorkload>> GetProfessorsWithWorkloadAsync(int page, int pageSize, string? sortField = null, string? sortOrder = "asc", CancellationToken cancellationToken = default);
}

/// <summary>
/// Profesor con carga de trabajo desde view_professors
/// </summary>
public sealed record ProfessorWithWorkload(
    int Id,
    string Name,
    string? Specialization,
    string? Email,
    string? Phone,
    bool IsActive,
    int TotalSubjects,
    int MaxAllowed,
    string Status
);
