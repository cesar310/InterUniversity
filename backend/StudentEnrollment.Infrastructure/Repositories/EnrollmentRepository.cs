using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Enums;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class EnrollmentRepository(ApplicationDbContext context) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Enrollments
            .Include(e => e.Student)
                .ThenInclude(s => s.User)
            .Include(e => e.Subject)
                .ThenInclude(s => s.Professor)
            .FirstOrDefaultAsync(e => e.StudentId == id || e.SubjectId == id, cancellationToken);

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        => await context.Enrollments
            .Include(e => e.Subject)
                .ThenInclude(s => s.Professor)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Enrollment>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        => await context.Enrollments
            .Include(e => e.Student)
                .ThenInclude(s => s.User)
            .Include(e => e.Subject)
                .ThenInclude(s => s.Professor)
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Enrollments.CountAsync(cancellationToken);

    public async Task<Enrollment> CreateAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await context.Enrollments.AddAsync(enrollment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return enrollment;
    }

    public async Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        context.Enrollments.Update(enrollment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int studentId, int subjectId, CancellationToken cancellationToken = default)
    {
        var enrollment = await context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId, cancellationToken);
        if (enrollment != null)
        {
            context.Enrollments.Remove(enrollment);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> HasActiveEnrollmentAsync(int studentId, int subjectId, CancellationToken cancellationToken = default)
        => await context.Enrollments.AnyAsync(
            e => e.StudentId == studentId 
                && e.SubjectId == subjectId 
                && e.Status == EnrollmentStatus.Active,
            cancellationToken);

    public async Task<int> CountActiveEnrollmentsBySubjectAsync(int subjectId, CancellationToken cancellationToken = default)
        => await context.Enrollments.CountAsync(
            e => e.SubjectId == subjectId && e.Status == EnrollmentStatus.Active,
            cancellationToken);

    public async Task<int> CountEnrollmentsByStudentAsync(int studentId, CancellationToken cancellationToken = default)
        => await context.Enrollments.CountAsync(e => e.StudentId == studentId, cancellationToken);

    public async Task<IEnumerable<Enrollment>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        => await context.Enrollments
            .Include(e => e.Student)
                .ThenInclude(s => s.User)
            .Where(e => e.SubjectId == subjectId)
            .OrderBy(e => e.Student.User.Email)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasActiveEnrollmentsBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        => await context.Enrollments.AnyAsync(
            e => e.SubjectId == subjectId && e.Status == EnrollmentStatus.Active,
            cancellationToken);

    public async Task<IEnumerable<ClassmateInfo>> GetClassmatesBySubjectAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        
        command.CommandText = @"
            SELECT subject_name, student_name 
            FROM view_classmates 
            WHERE subject_name = (SELECT name FROM subjects WHERE id = @subjectId)
            ORDER BY student_name";
        
        var subjectIdParam = command.CreateParameter();
        subjectIdParam.ParameterName = "@subjectId";
        subjectIdParam.Value = subjectId;
        command.Parameters.Add(subjectIdParam);
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var result = new List<ClassmateInfo>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new ClassmateInfo(
                SubjectName: reader.GetString(reader.GetOrdinal("subject_name")),
                StudentName: reader.GetString(reader.GetOrdinal("student_name"))
            ));
        }
        
        return result;
    }
}
