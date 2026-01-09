namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// Response de login con JWT token
/// </summary>
public sealed record LoginResponse(
    string Token,
    string TokenType,
    int ExpiresIn,
    UserInfoDto User
);

/// <summary>
/// Información del usuario autenticado
/// </summary>
public sealed record UserInfoDto(
    int Id,
    string Email,
    IEnumerable<string> Roles,
    int? StudentId,
    bool MustChangePassword,
    bool EmailVerified
);
