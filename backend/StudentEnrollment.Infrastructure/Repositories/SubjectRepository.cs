using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class SubjectRepository(ApplicationDbContext context) : ISubjectRepository
{
    public async Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Subjects
            .Include(s => s.Professor)
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Subject?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await context.Subjects
            .Include(s => s.Professor)
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Name == code, cancellationToken);

    public async Task<IEnumerable<Subject>> GetAllAsync(int page, int pageSize, string? sortField = null, string? sortOrder = "asc", CancellationToken cancellationToken = default)
    {
        var query = context.Subjects
            .Include(s => s.Professor)
            .Include(s => s.Enrollments)
            .AsQueryable();

        // Aplicar ordenamiento dinámico
        query = (sortField?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("name", "desc") => query.OrderByDescending(s => s.Name),
            ("name", _) => query.OrderBy(s => s.Name),
            ("credits", "desc") => query.OrderByDescending(s => s.Credits),
            ("credits", _) => query.OrderBy(s => s.Credits),
            ("professorname", "desc") => query.OrderByDescending(s => s.Professor.Name),
            ("professorname", _) => query.OrderBy(s => s.Professor.Name),
            ("enrolledstudents", "desc") => query.OrderByDescending(s => s.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active)),
            ("enrolledstudents", _) => query.OrderBy(s => s.Enrollments.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active)),
            ("isactive", "desc") => query.OrderByDescending(s => s.IsActive),
            ("isactive", _) => query.OrderBy(s => s.IsActive),
            _ => query.OrderBy(s => s.Name) // Default
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
        => await context.Subjects
            .Include(s => s.Professor)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Subjects.CountAsync(cancellationToken);

    public async Task<Subject> CreateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        await context.Subjects.AddAsync(subject, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return subject;
    }

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        context.Subjects.Update(subject);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var subject = await context.Subjects.FindAsync([id], cancellationToken);
        if (subject != null)
        {
            context.Subjects.Remove(subject);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
        => await context.Subjects.AnyAsync(s => s.Name == code, cancellationToken);

    public async Task<bool> CodeExistsExcludingSubjectAsync(string code, int subjectId, CancellationToken cancellationToken = default)
        => await context.Subjects.AnyAsync(s => s.Name == code && s.Id != subjectId, cancellationToken);

    public async Task<int> GetEnrollmentCountAsync(int subjectId, CancellationToken cancellationToken = default)
        => await context.Enrollments.CountAsync(e => e.SubjectId == subjectId, cancellationToken);

    public async Task<bool> SubjectNameExistsAsync(string name, CancellationToken cancellationToken = default)
        => await context.Subjects.AnyAsync(s => s.Name == name, cancellationToken);

    public async Task<int> CountByProfessorIdAsync(int professorId, CancellationToken cancellationToken = default)
        => await context.Subjects
            .Where(s => s.ProfessorId == professorId && s.IsActive)
            .CountAsync(cancellationToken);

    public async Task<IEnumerable<AcademicOfferItem>> GetAcademicOfferAsync(CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        
        command.CommandText = "SELECT subject_id, subject, description, credits, professor, specialization, professor_email, enrolled_students, available FROM view_academic_offer";
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var result = new List<AcademicOfferItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new AcademicOfferItem(
                SubjectId: reader.GetInt32(reader.GetOrdinal("subject_id")),
                Subject: reader.GetString(reader.GetOrdinal("subject")),
                Description: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                Credits: reader.GetInt32(reader.GetOrdinal("credits")),
                Professor: reader.GetString(reader.GetOrdinal("professor")),
                Specialization: reader.IsDBNull(reader.GetOrdinal("specialization")) ? null : reader.GetString(reader.GetOrdinal("specialization")),
                ProfessorEmail: reader.IsDBNull(reader.GetOrdinal("professor_email")) ? null : reader.GetString(reader.GetOrdinal("professor_email")),
                EnrolledStudents: Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("enrolled_students"))),
                Available: reader.GetBoolean(reader.GetOrdinal("available"))
            ));
        }
        
        return result;
    }
}
