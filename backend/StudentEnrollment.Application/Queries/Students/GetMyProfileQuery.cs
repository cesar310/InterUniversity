using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Students;

public sealed record GetMyProfileQuery(int StudentId) : IRequest<StudentDetailDto>;
