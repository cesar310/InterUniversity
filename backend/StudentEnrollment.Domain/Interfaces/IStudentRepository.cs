using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Domain.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Student?> GetByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Student> CreateAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> StudentCodeExistsAsync(string studentCode, CancellationToken cancellationToken = default);
    Task<bool> StudentCodeExistsExcludingStudentAsync(string studentCode, int studentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtiene estudiantes con carga académica desde view_student_enrollments
    /// </summary>
    Task<IEnumerable<StudentWithEnrollmentInfo>> GetStudentsWithEnrollmentsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Estudiante con información de inscripciones desde view_student_enrollments
/// </summary>
public sealed record StudentWithEnrollmentInfo(
    int StudentId,
    string StudentName,
    string StudentCode,
    string Email,
    bool IsActive,
    int EnrolledSubjects,
    int MaxAllowed,
    string? Subjects
);
