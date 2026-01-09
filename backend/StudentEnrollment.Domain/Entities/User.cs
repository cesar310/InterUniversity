namespace StudentEnrollment.Domain.Entities;

/// <summary>
/// Usuario del sistema (administradores y estudiantes)
/// </summary>
public sealed class User
{
    public int Id { get; set; }
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    // Email Verification
    public bool EmailVerified { get; private set; } = false;
    
    public string? EmailVerificationToken { get; private set; }
    
    public DateTime? EmailVerificationTokenExpiry { get; private set; }
    
    // Password Management
    public bool MustChangePassword { get; private set; } = false;
    
    public string? PasswordResetToken { get; private set; }
    
    public DateTime? PasswordResetTokenExpiry { get; private set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    
    public Student? Student { get; set; }
    
    // Domain Methods - Email Verification
    
    /// <summary>
    /// Genera un token único para verificación de email (válido por 24 horas)
    /// </summary>
    public void GenerateEmailVerificationToken()
    {
        EmailVerificationToken = Guid.NewGuid().ToString();
        EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
        EmailVerified = false;
    }
    
    /// <summary>
    /// Verifica el email del usuario con el token proporcionado
    /// </summary>
    /// <param name="token">Token de verificación enviado por email</param>
    /// <returns>True si la verificación fue exitosa, False si el token es inválido o expiró</returns>
    public bool VerifyEmail(string token)
    {
        if (string.IsNullOrWhiteSpace(token) ||
            EmailVerificationToken != token ||
            EmailVerificationTokenExpiry == null ||
            DateTime.UtcNow > EmailVerificationTokenExpiry)
        {
            return false;
        }
        
        EmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiry = null;
        return true;
    }
    
    // Domain Methods - Password Reset
    
    /// <summary>
    /// Genera un token único para reset de contraseña (válido por 1 hora)
    /// </summary>
    public void GeneratePasswordResetToken()
    {
        PasswordResetToken = Guid.NewGuid().ToString();
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
    }
    
    /// <summary>
    /// Valida si el token de reset de contraseña es correcto y no ha expirado
    /// </summary>
    /// <param name="token">Token de reset enviado por email</param>
    /// <returns>True si el token es válido, False si es inválido o expiró</returns>
    public bool ValidatePasswordResetToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;
            
        return PasswordResetToken == token &&
               PasswordResetTokenExpiry != null &&
               DateTime.UtcNow <= PasswordResetTokenExpiry;
    }
    
    /// <summary>
    /// Limpia el token de reset de contraseña después de usarlo exitosamente
    /// </summary>
    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }
    
    // Domain Methods - Password Management
    
    /// <summary>
    /// Marca que el usuario ya cambió su contraseña temporal
    /// </summary>
    public void MarkPasswordChanged()
    {
        MustChangePassword = false;
    }
    
    /// <summary>
    /// Fuerza al usuario a cambiar su contraseña en el próximo login
    /// (usado cuando se genera contraseña temporal)
    /// </summary>
    public void RequirePasswordChange()
    {
        MustChangePassword = true;
    }
    
    /// <summary>
    /// Requiere verificación de email
    /// </summary>
    public void RequireEmailVerification()
    {
        GenerateEmailVerificationToken();
    }
    
    /// <summary>
    /// Marca el email como verificado sin token (para usuarios creados por administradores)
    /// </summary>
    public void MarkEmailAsVerified()
    {
        EmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiry = null;
    }
}
