namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de materia
/// </summary>
public sealed record SubjectDto(
    int Id,
    string Name,
    string? Description,
    int Credits,
    int ProfessorId,
    string ProfessorName,
    int EnrolledStudents,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// DTO de materia con detalles del profesor
/// </summary>
public sealed record SubjectDetailDto(
    int Id,
    string Name,
    string? Description,
    int Credits,
    int ProfessorId,
    string ProfessorName,
    string? ProfessorSpecialization,
    string? ProfessorEmail,
    int EnrolledStudents,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// Response paginado de materias
/// </summary>
public sealed record PagedSubjectsResponse(
    IEnumerable<SubjectDto> Subjects,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
