using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Domain.Interfaces;

/// <summary>
/// Service for generating JWT tokens
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the specified user with their roles
    /// </summary>
    string GenerateToken(User user, IEnumerable<string> roles);
}
