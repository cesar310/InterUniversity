using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Common;

namespace StudentEnrollment.Application.Commands.Auth;

/// <summary>
/// Command para registrar nuevo estudiante
/// </summary>
public sealed record RegisterStudentCommand(
    string Email,
    string Name
) : IRequest<Result<RegisterStudentResponse>>;
