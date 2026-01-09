using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Students;

public sealed record GetStudentByIdQuery(int StudentId) : IRequest<StudentDetailDto>;
