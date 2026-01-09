namespace StudentEnrollment.Domain.Interfaces;

/// <summary>
/// Servicio para hashing y verificación de contraseñas
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera hash de contraseña con BCrypt
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si la contraseña coincide con el hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Genera contraseña temporal aleatoria (12 caracteres: mayúsculas, minúsculas, números, símbolos)
    /// </summary>
    string GenerateTemporaryPassword();
}
