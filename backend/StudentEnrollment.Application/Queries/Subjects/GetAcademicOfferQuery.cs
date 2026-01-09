using MediatR;
using StudentEnrollment.Application.DTOs;

namespace StudentEnrollment.Application.Queries.Subjects;

/// <summary>
/// Query para obtener la oferta académica desde view_academic_offer
/// </summary>
public sealed record GetAcademicOfferQuery() : IRequest<IEnumerable<AcademicOfferDto>>;
