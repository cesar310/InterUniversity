using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Professors;

public sealed record GetAllProfessorsQuery(
    int Page = 1,
    int PageSize = 10,
    bool? IsActive = true
) : IRequest<PagedProfessorsResponse>;
