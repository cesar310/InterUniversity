namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de estadísticas del dashboard
/// </summary>
public sealed record DashboardStatsDto(
    int TotalStudents,
    int TotalProfessors,
    int ActiveSubjects,
    int ActiveEnrollments,
    decimal AvgSubjectsPerStudent,
    int MaxSubjectsEnrolled
);
