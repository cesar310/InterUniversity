using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Commands.Subjects;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.Subjects;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SubjectsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtener todas las materias (paginado)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedSubjectsResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = true,
        [FromQuery] int? professorId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllSubjectsQuery(page, pageSize, isActive, professorId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedSubjectsResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtener materia por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SubjectDetailDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetSubjectByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<SubjectDetailDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Crear nueva materia - Solo administrador
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        var subjectId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = subjectId },
            ApiResponse<int>.SuccessResponse(subjectId, "Materia creada exitosamente")
        );
    }

    /// <summary>
    /// Actualizar materia - Solo administrador
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
        int id,
        [FromBody] UpdateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.SubjectId)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("El ID no coincide", "ID_MISMATCH"));
        }

        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse("Materia actualizada exitosamente", "Materia actualizada exitosamente"));
    }

    /// <summary>
    /// Eliminar materia - Solo administrador
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSubjectCommand(id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Obtener oferta académica completa desde view_academic_offer
    /// Muestra materias con profesor, créditos y estudiantes inscritos
    /// </summary>
    [HttpGet("academic-offer")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AcademicOfferDto>>>> GetAcademicOffer(
        CancellationToken cancellationToken)
    {
        var query = new GetAcademicOfferQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<AcademicOfferDto>>.SuccessResponse(result));
    }
}
