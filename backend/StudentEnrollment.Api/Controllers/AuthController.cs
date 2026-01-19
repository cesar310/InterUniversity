using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Application.Commands.Auth;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Application.Queries.Auth;

namespace StudentEnrollment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Login de usuario (estudiante o administrador)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResponse(
                result.Error ?? "Error de autenticación",
                result.ErrorCode
            ));
        }

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(
            result.Value!, 
            "Inicio de sesión exitoso"
        ));
    }

    /// <summary>
    /// Registro de estudiante por cuenta propia
    /// </summary>
    [HttpPost("self-register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<SelfRegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<SelfRegisterResponse>>> SelfRegister(
        [FromBody] SelfRegisterStudentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            var statusCode = result.ErrorCode switch
            {
                "EMAIL_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
                "STUDENT_CODE_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };
            
            return StatusCode(statusCode, ApiResponse<object>.ErrorResponse(
                result.Error ?? "Error en el registro",
                result.ErrorCode
            ));
        }

        return CreatedAtAction(
            nameof(SelfRegister),
            new { id = result.Value!.StudentId },
            ApiResponse<SelfRegisterResponse>.SuccessResponse(result.Value, "Estudiante registrado exitosamente")
        );
    }

    /// <summary>
    /// Registro de nuevo estudiante (solo administrador)
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "administrator")]
    [ProducesResponseType(typeof(ApiResponse<RegisterStudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<RegisterStudentResponse>>> Register(
        [FromBody] RegisterStudentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            var statusCode = result.ErrorCode switch
            {
                "EMAIL_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
                "ROLE_NOT_FOUND" => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };
            
            return StatusCode(statusCode, ApiResponse<object>.ErrorResponse(
                result.Error ?? "Error en el registro",
                result.ErrorCode
            ));
        }
        
        return CreatedAtAction(
            nameof(Register),
            new { id = result.Value!.StudentId },
            ApiResponse<RegisterStudentResponse>.SuccessResponse(result.Value, "Estudiante registrado exitosamente")
        );
    }

    /// <summary>
    /// Verificar email con token
    /// </summary>
    [HttpGet("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<VerifyEmailResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<VerifyEmailResponse>>> VerifyEmail(
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        var query = new VerifyEmailQuery(token);
        var result = await mediator.Send(query, cancellationToken);
        
        return Ok(ApiResponse<VerifyEmailResponse>.SuccessResponse(
            result.Value!, 
            "Verificación de email procesada"
        ));
    }

    /// <summary>
    /// Reenviar email de verificación
    /// </summary>
    [HttpPost("email-verifications")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ResendEmailVerificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ResendEmailVerificationResponse>>> ResendEmailVerification(
        [FromBody] ResendEmailVerificationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                result.Error ?? "Error al reenviar la verificación",
                result.ErrorCode
            ));
        }

        return Ok(ApiResponse<ResendEmailVerificationResponse>.SuccessResponse(
            result.Value!, 
            "Email de verificación enviado"
        ));
    }

    /// <summary>
    /// Olvido de contraseña - Generar contraseña temporal
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ForgotPasswordResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ForgotPasswordResponse>>> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        return Ok(ApiResponse<ForgotPasswordResponse>.SuccessResponse(
            result.Value!, 
            "Recuperación de contraseña procesada"
        ));
    }

    /// <summary>
    /// Cambiar contraseña (requiere autenticación)
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<string>.SuccessResponse("Contraseña cambiada exitosamente", "Contraseña cambiada exitosamente"));
    }
}
