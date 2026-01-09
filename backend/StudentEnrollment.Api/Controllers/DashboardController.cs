using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.Dashboard;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize(Roles = "administrator")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtener estadísticas del sistema - Solo administrador
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats(
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardStatsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(result));
    }
}
