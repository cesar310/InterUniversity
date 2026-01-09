using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de inscripción
/// </summary>
public sealed record EnrollmentDto(
    int StudentId,
    int SubjectId,
    string StudentCode,
    string StudentEmail,
    string SubjectName,
    string ProfessorName,
    EnrollmentStatus Status,
    DateTime EnrolledAt,
    DateTime UpdatedAt
);

/// <summary>
/// DTO simple de inscripción para estudiante
/// </summary>
public sealed record MyEnrollmentDto(
    int SubjectId,
    string SubjectName,
    int Credits,
    string ProfessorName,
    EnrollmentStatus Status,
    DateTime EnrolledAt
);

/// <summary>
/// Response de mis inscripciones
/// </summary>
public sealed record MyEnrollmentsResponse(
    int StudentId,
    string StudentName,
    IEnumerable<MyEnrollmentDto> Enrollments,
    int TotalCredits,
    int ActiveEnrollments,
    int MaxAllowed
);

/// <summary>
/// Response paginado de inscripciones
/// </summary>
public sealed record PagedEnrollmentsResponse(
    IEnumerable<EnrollmentDto> Enrollments,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
