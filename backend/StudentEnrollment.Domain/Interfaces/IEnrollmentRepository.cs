using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Domain.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Enrollment> CreateAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int studentId, int subjectId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveEnrollmentAsync(int studentId, int subjectId, CancellationToken cancellationToken = default);
    Task<int> CountActiveEnrollmentsBySubjectAsync(int subjectId, CancellationToken cancellationToken = default);
    Task<int> CountEnrollmentsByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveEnrollmentsBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene compañeros de clase desde view_classmates filtrado por materia
    /// </summary>
    Task<IEnumerable<ClassmateInfo>> GetClassmatesBySubjectAsync(int subjectId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Información de compañeros de clase desde view_classmates
/// </summary>
public sealed record ClassmateInfo(
    string SubjectName,
    string StudentName
);
