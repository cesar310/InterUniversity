using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Commands.SystemConfigs;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.SystemConfigs;
using System.Security.Claims;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/config")]
[Authorize]
public class SystemConfigController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtener todas las configuraciones del sistema - Requiere autenticación
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SystemConfigDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetAllSystemConfigsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<SystemConfigDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtener configuración por clave - Requiere autenticación
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<ApiResponse<SystemConfigDto>>> GetByKey(
        string key,
        CancellationToken cancellationToken)
    {
        var query = new GetSystemConfigByKeyQuery(key);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<SystemConfigDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Actualizar configuración - Solo administrador
    /// </summary>
    [HttpPatch("{key}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
        string key,
        [FromBody] UpdateSystemConfigRequest request,
        CancellationToken cancellationToken)
    {
        // Obtener UserId del token - JWT usa el namespace completo para 'sub'
        var userIdClaim = User.Claims.FirstOrDefault(c => 
            c.Type == ClaimTypes.NameIdentifier || 
            c.Type == "sub" || 
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("ID de usuario inválido en el token", "INVALID_TOKEN"));
        }

        var command = new UpdateSystemConfigCommand(key, request.Value, userId);
        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse("Configuración actualizada exitosamente", "Configuración actualizada exitosamente"));
    }

    /// <summary>
    /// Obtener historial de auditoría de configuraciones desde view_config_audit - Solo administrador
    /// </summary>
    [HttpGet("audit")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ConfigAuditDto>>>> GetAuditLog(
        CancellationToken cancellationToken)
    {
        var query = new GetConfigAuditLogQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<ConfigAuditDto>>.SuccessResponse(result));
    }
}

/// <summary>
/// Request para actualizar configuración
/// </summary>
public sealed record UpdateSystemConfigRequest(string Value);
