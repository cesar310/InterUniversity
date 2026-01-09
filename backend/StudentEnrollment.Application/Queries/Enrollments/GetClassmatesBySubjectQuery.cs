using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Enrollments;

/// <summary>
/// Query para obtener compañeros de clase desde view_classmates
/// </summary>
public sealed record GetClassmatesBySubjectQuery(int SubjectId) : IRequest<IEnumerable<ClassmateDto>>;
