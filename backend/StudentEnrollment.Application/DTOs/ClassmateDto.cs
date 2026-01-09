namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// DTO de compañeros de clase desde view_classmates
/// </summary>
public sealed record ClassmateDto(
    string SubjectName,
    string StudentName
);
