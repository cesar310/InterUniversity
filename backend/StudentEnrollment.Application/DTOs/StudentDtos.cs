namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de estudiante
/// </summary>
public sealed record StudentDto(
    int Id,
    string StudentCode,
    int UserId,
    string Name,
    string Email,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// DTO de estudiante con detalles completos
/// </summary>
public sealed record StudentDetailDto(
    int Id,
    string StudentCode,
    int UserId,
    string Email,
    bool IsActive,
    bool EmailVerified,
    bool MustChangePassword,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Response de registro de estudiante (incluye contraseña temporal)
/// </summary>
public sealed record RegisterStudentResponse(
    int StudentId,
    int UserId,
    string StudentCode,
    string Email,
    string TemporaryPassword
);

/// <summary>
/// Response paginado de estudiantes
/// </summary>
public sealed record PagedStudentsResponse(
    IEnumerable<StudentDto> Students,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
