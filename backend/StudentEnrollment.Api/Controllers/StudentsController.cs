using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Commands.Students;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.Students;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class StudentsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtener todos los estudiantes (paginado) - Solo administrador
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<PagedStudentsResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllStudentsQuery(page, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedStudentsResponse>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtener estudiante por ID - Solo administrador
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<StudentDetailDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<StudentDetailDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtener perfil del estudiante autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<StudentDetailDto>>> GetMyProfile(
        CancellationToken cancellationToken)
    {
        // Obtener StudentId del token JWT
        var studentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StudentId")?.Value;
        if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
        {
            return BadRequest(ApiResponse<StudentDetailDto>.ErrorResponse("ID de estudiante inválido en el token", "INVALID_TOKEN"));
        }

        var query = new GetMyProfileQuery(studentId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<StudentDetailDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Actualizar estudiante - Solo administrador
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
        int id,
        [FromBody] UpdateStudentCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.StudentId)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("El ID no coincide", "ID_MISMATCH"));
        }

        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse("Estudiante actualizado exitosamente", "Estudiante actualizado exitosamente"));
    }

    /// <summary>
    /// Eliminar estudiante - Solo administrador
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteStudentCommand(id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Obtener estudiantes con carga académica desde view_student_enrollments
    /// Muestra estudiantes con materias inscritas y límites - Solo administrador
    /// </summary>
    [HttpGet("with-enrollments")]
    [Authorize(Roles = "administrator")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StudentWithEnrollmentsDto>>>> GetWithEnrollments(
        CancellationToken cancellationToken)
    {
        var query = new GetStudentsWithEnrollmentsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<StudentWithEnrollmentsDto>>.SuccessResponse(result));
    }
}
