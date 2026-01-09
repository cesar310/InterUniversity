using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Commands.Professors;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.Professors;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "administrator")]
public class ProfessorsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtener todos los profesores (paginado) - Solo administrador
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedProfessorsResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProfessorsQuery(page, pageSize, isActive);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedProfessorsResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtener profesor por ID - Solo administrador
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProfessorDetailDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetProfessorByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<ProfessorDetailDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Crear nuevo profesor - Solo administrador
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateProfessorCommand command,
        CancellationToken cancellationToken)
    {
        var professorId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = professorId },
            ApiResponse<int>.SuccessResponse(professorId, "Profesor creado exitosamente")
        );
    }

    /// <summary>
    /// Actualizar profesor - Solo administrador
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
        int id,
        [FromBody] UpdateProfessorCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.ProfessorId)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("El ID no coincide", "ID_MISMATCH"));
        }

        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse("Profesor actualizado exitosamente", "Profesor actualizado exitosamente"));
    }

    /// <summary>
    /// Eliminar profesor - Solo administrador
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProfessorCommand(id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
