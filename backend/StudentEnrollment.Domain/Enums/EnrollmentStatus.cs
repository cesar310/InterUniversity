namespace StudentEnrollment.Domain.Enums;

/// <summary>
/// Estado de la inscripción de un estudiante a una materia
/// </summary>
public enum EnrollmentStatus
{
    /// <summary>
    /// Inscripción activa (estudiante cursando)
    /// </summary>
    Active,
    
    /// <summary>
    /// Materia completada exitosamente
    /// </summary>
    Completed,
    
    /// <summary>
    /// Inscripción cancelada
    /// </summary>
    Cancelled
}
