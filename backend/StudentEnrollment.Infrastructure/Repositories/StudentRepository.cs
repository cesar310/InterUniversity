using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class StudentRepository(ApplicationDbContext context) : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Students
            .Include(s => s.User)
                .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Students
            .Include(s => s.User)
                .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

    public async Task<Student?> GetByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default)
        => await context.Students
            .Include(s => s.User)
                .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(s => s.StudentCode == studentCode, cancellationToken);

    public async Task<IEnumerable<Student>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        => await context.Students
            .Include(s => s.User)
            .OrderBy(s => s.User.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Students.CountAsync(cancellationToken);

    public async Task<Student> CreateAsync(Student student, CancellationToken cancellationToken = default)
    {
        await context.Students.AddAsync(student, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return student;
    }

    public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        context.Students.Update(student);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await context.Students.FindAsync([id], cancellationToken);
        if (student != null)
        {
            context.Students.Remove(student);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> StudentCodeExistsAsync(string studentCode, CancellationToken cancellationToken = default)
        => await context.Students.AnyAsync(s => s.StudentCode == studentCode, cancellationToken);

    public async Task<bool> StudentCodeExistsExcludingStudentAsync(string studentCode, int studentId, CancellationToken cancellationToken = default)
        => await context.Students.AnyAsync(s => s.StudentCode == studentCode && s.Id != studentId, cancellationToken);

    public async Task<IEnumerable<StudentWithEnrollmentInfo>> GetStudentsWithEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        
        command.CommandText = @"
            SELECT student_id, student_name, student_code, email, is_active, enrolled_subjects, max_allowed, subjects 
            FROM view_student_enrollments 
            ORDER BY student_name";
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var result = new List<StudentWithEnrollmentInfo>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new StudentWithEnrollmentInfo(
                StudentId: reader.GetInt32(reader.GetOrdinal("student_id")),
                StudentName: reader.GetString(reader.GetOrdinal("student_name")),
                StudentCode: reader.GetString(reader.GetOrdinal("student_code")),
                Email: reader.GetString(reader.GetOrdinal("email")),
                IsActive: reader.GetBoolean(reader.GetOrdinal("is_active")),
                EnrolledSubjects: Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("enrolled_subjects"))),
                MaxAllowed: reader.GetInt32(reader.GetOrdinal("max_allowed")),
                Subjects: reader.IsDBNull(reader.GetOrdinal("subjects")) ? null : reader.GetString(reader.GetOrdinal("subjects"))
            ));
        }
        
        return result;
    }
}
