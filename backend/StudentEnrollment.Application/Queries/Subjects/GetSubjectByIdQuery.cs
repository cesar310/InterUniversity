using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Subjects;

public sealed record GetSubjectByIdQuery(int SubjectId) : IRequest<SubjectDetailDto>;
