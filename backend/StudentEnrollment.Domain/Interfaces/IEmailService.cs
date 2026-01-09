namespace StudentEnrollment.Domain.Interfaces;

/// <summary>
/// Servicio para envío de emails transaccionales (contraseñas temporales, verificación, notificaciones)
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envía email con contraseña temporal a estudiante recién registrado
    /// </summary>
    Task<bool> SendTemporaryPasswordEmailAsync(string toEmail, string studentName, string temporaryPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía email con link de verificación de cuenta
    /// </summary>
    Task<bool> SendEmailVerificationAsync(string toEmail, string studentName, string verificationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía email con link para resetear contraseña
    /// </summary>
    Task<bool> SendPasswordResetEmailAsync(string toEmail, string studentName, string resetToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía notificación de matrícula exitosa
    /// </summary>
    Task<bool> SendEnrollmentConfirmationAsync(string toEmail, string studentName, string subjectName, CancellationToken cancellationToken = default);
}
