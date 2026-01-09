using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Queries.Dashboard;

public sealed class GetDashboardStatsQueryHandler(
    IDashboardRepository dashboardRepository)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(
        GetDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await dashboardRepository.GetSystemStatisticsAsync(cancellationToken);
        
        // Mapear del modelo de dominio al DTO
        return new DashboardStatsDto(
            TotalStudents: stats.TotalStudents,
            TotalProfessors: stats.TotalProfessors,
            ActiveSubjects: stats.ActiveSubjects,
            ActiveEnrollments: stats.ActiveEnrollments,
            AvgSubjectsPerStudent: stats.AvgSubjectsPerStudent,
            MaxSubjectsEnrolled: stats.MaxSubjectsEnrolled
        );
    }
}
