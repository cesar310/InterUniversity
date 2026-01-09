using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Professors;

public sealed record GetProfessorByIdQuery(int ProfessorId) : IRequest<ProfessorDetailDto>;
