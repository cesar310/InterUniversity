using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Dashboard;

/// <summary>
/// Query para obtener estadísticas del dashboard
/// </summary>
public sealed record GetDashboardStatsQuery() : IRequest<DashboardStatsDto>;
