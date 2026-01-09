namespace StudentEnrollment.Domain.Interfaces;

public interface IDashboardRepository
{
    /// <summary>
    /// Obtiene las estadísticas del sistema llamando al stored procedure get_system_statistics
    /// </summary>
    Task<DashboardStats> GetSystemStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Estadísticas del sistema para el dashboard
/// </summary>
public sealed record DashboardStats(
    int TotalStudents,
    int TotalProfessors,
    int ActiveSubjects,
    int ActiveEnrollments,
    decimal AvgSubjectsPerStudent,
    int MaxSubjectsEnrolled
);
