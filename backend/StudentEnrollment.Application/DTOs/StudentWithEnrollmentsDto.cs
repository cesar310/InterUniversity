namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de estudiante con carga académica desde view_student_enrollments
/// </summary>
public sealed record StudentWithEnrollmentsDto(
    int StudentId,
    string StudentName,
    string StudentCode,
    string Email,
    bool IsActive,
    int EnrolledSubjects,
    int MaxAllowed,
    string? Subjects
);
