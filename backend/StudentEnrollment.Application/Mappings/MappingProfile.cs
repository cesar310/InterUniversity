using AutoMapper;
using StudentEnrollment.Application.DTOs;
using StudentEnrollment.Domain.Entities;

namespace StudentEnrollment.Application.Mappings;

/// <summary>
/// Perfil de mapeo de AutoMapper para convertir entidades a DTOs
/// </summary>
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student mappings
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.StudentCode))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.User.UpdatedAt));

        CreateMap<Student, StudentDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.StudentCode))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive))
            .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => src.User.EmailVerified))
            .ForMember(dest => dest.MustChangePassword, opt => opt.MapFrom(src => src.User.MustChangePassword))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.User.UpdatedAt));

        // Subject mappings
        CreateMap<Subject, SubjectDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Credits))
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.ProfessorName, opt => opt.MapFrom(src => src.Professor.Name))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.EnrolledStudents, opt => opt.MapFrom(src => src.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active)));

        CreateMap<Subject, SubjectDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Credits))
            .ForMember(dest => dest.ProfessorId, opt => opt.MapFrom(src => src.ProfessorId))
            .ForMember(dest => dest.ProfessorName, opt => opt.MapFrom(src => src.Professor.Name))
            .ForMember(dest => dest.ProfessorEmail, opt => opt.MapFrom(src => src.Professor.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.EnrolledStudents, opt => opt.MapFrom(src => src.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        // Professor mappings
        CreateMap<Professor, ProfessorDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.TotalSubjects, opt => opt.MapFrom(src => src.Subjects.Count(s => s.IsActive)))
            .ForMember(dest => dest.MaxAllowed, opt => opt.Ignore()) // Se asigna en el handler
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<Professor, ProfessorDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.TotalSubjects, opt => opt.MapFrom(src => src.Subjects.Count(s => s.IsActive)))
            .ForMember(dest => dest.MaxAllowed, opt => opt.Ignore()) // Se asigna en el handler
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Enrollment mappings
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.StudentCode))
            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.User.Email))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
            .ForMember(dest => dest.ProfessorName, opt => opt.MapFrom(src => src.Subject.Professor.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<Enrollment, MyEnrollmentDto>()
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
            .ForMember(dest => dest.ProfessorName, opt => opt.MapFrom(src => src.Subject.Professor.Name))
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Subject.Credits))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.EnrolledAt));

        // SystemConfig mappings
        CreateMap<SystemConfig, SystemConfigDto>()
            .ForMember(dest => dest.ConfigKey, opt => opt.MapFrom(src => src.ConfigKey))
            .ForMember(dest => dest.ConfigValue, opt => opt.MapFrom(src => src.ConfigValue))
            .ForMember(dest => dest.ValueType, opt => opt.MapFrom(src => src.ValueType.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsEditable, opt => opt.MapFrom(src => src.IsEditable));

        // User mappings
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name)))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student != null ? src.Student.Id : (int?)null))
            .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => src.EmailVerified))
            .ForMember(dest => dest.MustChangePassword, opt => opt.MapFrom(src => src.MustChangePassword));
    }
}
