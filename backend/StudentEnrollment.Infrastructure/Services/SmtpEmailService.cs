using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Infrastructure.Services;

/// <summary>
/// Servicio de email usando SMTP directo (Gmail, Outlook, etc.)
/// Similar a Flask-Mail en Python
/// </summary>
public sealed class SmtpEmailService(
    IConfiguration configuration,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly string _smtpHost = configuration["SmtpSettings:Host"] ?? "smtp.gmail.com";
    private readonly int _smtpPort = int.Parse(configuration["SmtpSettings:Port"] ?? "587");
    private readonly string _smtpUser = configuration["SmtpSettings:Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
    private readonly string _smtpPassword = configuration["SmtpSettings:Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
    private readonly string _senderEmail = configuration["SmtpSettings:SenderEmail"] ?? configuration["SmtpSettings:Username"] ?? "noreply@sistema.com";
    private readonly string _senderName = configuration["SmtpSettings:SenderName"] ?? "Sistema de Matrícula";
    private readonly string _frontendUrl = configuration["Frontend:DevTunnelUrl"] ?? configuration["Frontend:Url"] ?? "http://localhost:4200";

    public async Task<bool> SendTemporaryPasswordEmailAsync(string toEmail, string studentName, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Enviando email de contraseña temporal a {Email} via SMTP", toEmail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress(studentName, toEmail));
            message.Subject = "Bienvenido - Tu contraseña temporal";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                            .content {{ background-color: #f9f9f9; padding: 20px; }}
                            .password {{ background-color: #fff; border: 2px solid #4CAF50; padding: 15px; font-size: 24px; font-weight: bold; text-align: center; margin: 20px 0; }}
                            .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>¡Bienvenido, {studentName}!</h1>
                            </div>
                            <div class='content'>
                                <p>Tu cuenta ha sido creada exitosamente en el Sistema de Inscripción Estudiantil.</p>
                                <p>Tu contraseña temporal es:</p>
                                <div class='password'>{temporaryPassword}</div>
                                <p><strong>⚠️ Importante:</strong> Por seguridad, debes cambiar esta contraseña en tu primer inicio de sesión.</p>
                            </div>
                            <div class='footer'>
                                <p>Este es un correo automático, por favor no responder.</p>
                            </div>
                        </div>
                    </body>
                    </html>",
                TextBody = $@"
                    Bienvenido, {studentName}!
                    
                    Tu cuenta ha sido creada exitosamente.
                    
                    Tu contraseña temporal es: {temporaryPassword}
                    
                    Por seguridad, debes cambiar esta contraseña en tu primer inicio de sesión."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_smtpUser, _smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email de contraseña temporal enviado exitosamente a {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar email de contraseña temporal a {Email}. SMTP: {Host}:{Port}", toEmail, _smtpHost, _smtpPort);
            return false;
        }
    }

    public async Task<bool> SendEmailVerificationAsync(string toEmail, string studentName, string verificationToken, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Enviando email de verificación a {Email} via SMTP", toEmail);

            var verificationLink = $"{_frontendUrl}/auth/verify-email?token={verificationToken}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress(studentName, toEmail));
            message.Subject = "Verifica tu correo electrónico";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h1>Verifica tu correo, {studentName}</h1>
                        <p>Por favor, verifica tu correo electrónico haciendo clic en el siguiente enlace:</p>
                        <p><a href='{verificationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Verificar Email</a></p>
                        <p><small>Este enlace expirará en 24 horas.</small></p>
                    </body>
                    </html>",
                TextBody = $"Verifica tu correo, {studentName}\n\nPor favor, verifica tu correo visitando: {verificationLink}\n\nEste enlace expirará en 24 horas."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_smtpUser, _smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email de verificación enviado exitosamente a {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar email de verificación a {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string studentName, string resetToken, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Enviando email de recuperación de contraseña a {Email} via SMTP", toEmail);

            var resetLink = $"{_frontendUrl}/auth/reset-password?token={resetToken}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress(studentName, toEmail));
            message.Subject = "Recuperación de Contraseña";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h1>Recuperación de Contraseña</h1>
                        <p>Hola {studentName},</p>
                        <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                        <p><a href='{resetLink}' style='background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Restablecer Contraseña</a></p>
                        <p><small>Este enlace expirará en 1 hora.</small></p>
                        <p>Si no solicitaste este cambio, puedes ignorar este correo.</p>
                    </body>
                    </html>",
                TextBody = $"Recuperación de Contraseña\n\nHola {studentName},\n\nPara restablecer tu contraseña, visita: {resetLink}\n\nEste enlace expirará en 1 hora."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_smtpUser, _smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email de recuperación de contraseña enviado exitosamente a {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar email de recuperación de contraseña a {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendEnrollmentConfirmationAsync(string toEmail, string studentName, string subjectName, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Enviando email de confirmación de inscripción a {Email} via SMTP", toEmail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress(studentName, toEmail));
            message.Subject = "Confirmación de Inscripción";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h1>¡Inscripción Exitosa!</h1>
                        <p>Hola {studentName},</p>
                        <p>Te has inscrito exitosamente a:</p>
                        <h2 style='color: #4CAF50;'>{subjectName}</h2>
                        <p>¡Mucho éxito en tu curso!</p>
                    </body>
                    </html>",
                TextBody = $"Inscripción Exitosa\n\nHola {studentName},\n\nTe has inscrito exitosamente a: {subjectName}\n\n¡Mucho éxito en tu curso!"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_smtpUser, _smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email de confirmación de inscripción enviado exitosamente a {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar email de confirmación de inscripción a {Email}", toEmail);
            return false;
        }
    }
}
