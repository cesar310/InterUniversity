using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Commands.Enrollments;

public sealed record EnrollStudentCommand(
    int StudentId,
    int SubjectId
) : IRequest<EnrollmentDto>;
