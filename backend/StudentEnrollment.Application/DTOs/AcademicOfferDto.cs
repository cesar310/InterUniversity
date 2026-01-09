namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de oferta académica desde view_academic_offer
/// </summary>
public sealed record AcademicOfferDto(
    int SubjectId,
    string Subject,
    string? Description,
    int Credits,
    string Professor,
    string? Specialization,
    string? ProfessorEmail,
    int EnrolledStudents,
    bool Available
);
