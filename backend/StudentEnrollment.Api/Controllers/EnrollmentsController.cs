using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Commands.Enrollments;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.Enrollments;
using StudentEnrollment.Domain.Enums;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EnrollmentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Inscribirse en una materia - Solo estudiante
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Enroll(
        [FromBody] EnrollStudentCommand command,
        CancellationToken cancellationToken)
    {
        // Verificar que el StudentId del comando coincida con el del token
        var studentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var tokenStudentId))
        {
            return BadRequest(ApiResponse<EnrollmentDto>.ErrorResponse("ID de estudiante inválido en el token", "INVALID_TOKEN"));
        }

        if (command.StudentId != tokenStudentId)
        {
            return Forbid();
        }

        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetMyEnrollments),
            null,
            ApiResponse<EnrollmentDto>.SuccessResponse(result, "Inscripción exitosa")
        );
    }

    /// <summary>
    /// Obtener mis inscripciones - Solo estudiante
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<MyEnrollmentsResponse>>> GetMyEnrollments(
        [FromQuery] EnrollmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var studentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
        {
            return BadRequest(ApiResponse<MyEnrollmentsResponse>.ErrorResponse("ID de estudiante inválido en el token", "INVALID_TOKEN"));
        }

        var query = new GetMyEnrollmentsQuery(studentId, status);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<MyEnrollmentsResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtener todas las inscripciones (paginado) - Solo administrador
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<PagedEnrollmentsResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? studentId = null,
        [FromQuery] int? subjectId = null,
        [FromQuery] EnrollmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllEnrollmentsQuery(page, pageSize, studentId, subjectId, status);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedEnrollmentsResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Eliminar inscripción - Estudiante (propia) o Administrador
    /// </summary>
    [HttpDelete("{studentId}/{subjectId}")]
    public async Task<ActionResult> Delete(
        int studentId,
        int subjectId,
        CancellationToken cancellationToken)
    {
        // Verificar permisos
        var isAdmin = User.IsInRole("administrator");
        var studentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        var tokenStudentId = string.IsNullOrEmpty(studentIdClaim) ? 0 : int.Parse(studentIdClaim);

        if (!isAdmin && tokenStudentId != studentId)
        {
            return Forbid();
        }

        var command = new DeleteEnrollmentCommand(studentId, subjectId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Obtener compañeros de clase por materia desde view_classmates
    /// Muestra estudiantes inscritos en una materia específica
    /// </summary>
    [HttpGet("classmates/{subjectId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClassmateDto>>>> GetClassmates(
        int subjectId,
        CancellationToken cancellationToken)
    {
        var query = new GetClassmatesBySubjectQuery(subjectId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<ClassmateDto>>.SuccessResponse(result));
    }
}
