using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Domain.Interfaces;

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Subject?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetAllAsync(int page, int pageSize, string? sortField = null, string? sortOrder = "asc", CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Subject> CreateAsync(Subject subject, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsExcludingSubjectAsync(string code, int subjectId, CancellationToken cancellationToken = default);
    Task<int> GetEnrollmentCountAsync(int subjectId, CancellationToken cancellationToken = default);
    Task<bool> SubjectNameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<int> CountByProfessorIdAsync(int professorId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene la oferta académica desde la vista view_academic_offer
    /// </summary>
    Task<IEnumerable<AcademicOfferItem>> GetAcademicOfferAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Item de oferta académica desde view_academic_offer
/// </summary>
public sealed record AcademicOfferItem(
    int SubjectId,
    string Subject,
    string? Description,
    int Credits,
    string Professor,
    string? Specialization,
    string? ProfessorEmail,
    int EnrolledStudents,
    bool Available
);
