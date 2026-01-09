namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de profesor
/// </summary>
public sealed record ProfessorDto(
    int Id,
    string Name,
    string? Specialization,
    string? Email,
    string? Phone,
    bool IsActive,
    int TotalSubjects,
    int MaxAllowed,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// DTO de profesor con detalle completo
/// </summary>
public sealed record ProfessorDetailDto(
    int Id,
    string Name,
    string? Specialization,
    string? Email,
    string? Phone,
    bool IsActive,
    int TotalSubjects,
    int MaxAllowed,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Response paginado de profesores
/// </summary>
public sealed record PagedProfessorsResponse(
    IEnumerable<ProfessorDto> Professors,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
