using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Students;

/// <summary>
/// Query para obtener estudiantes con carga académica desde view_student_enrollments
/// </summary>
public sealed record GetStudentsWithEnrollmentsQuery() : IRequest<IEnumerable<StudentWithEnrollmentsDto>>;
