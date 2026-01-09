using MediatR;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Interfaces;

namespace StudentEnrollment.Application.Queries.Subjects;

public sealed class GetAcademicOfferQueryHandler(
    ISubjectRepository subjectRepository)
    : IRequestHandler<GetAcademicOfferQuery, IEnumerable<AcademicOfferDto>>
{
    public async Task<IEnumerable<AcademicOfferDto>> Handle(
        GetAcademicOfferQuery request,
        CancellationToken cancellationToken)
    {
        var academicOffer = await subjectRepository.GetAcademicOfferAsync(cancellationToken);
        
        return academicOffer.Select(item => new AcademicOfferDto(
            SubjectId: item.SubjectId,
            Subject: item.Subject,
            Description: item.Description,
            Credits: item.Credits,
            Professor: item.Professor,
            Specialization: item.Specialization,
            ProfessorEmail: item.ProfessorEmail,
            EnrolledStudents: item.EnrolledStudents,
            Available: item.Available
        )).ToList();
    }
}
