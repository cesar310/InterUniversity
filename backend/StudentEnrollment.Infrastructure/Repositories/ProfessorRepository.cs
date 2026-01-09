using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class ProfessorRepository(ApplicationDbContext context) : IProfessorRepository
{
    public async Task<Professor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Professors
            .Include(p => p.Subjects)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Professor?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken = default)
        => await context.Professors
            .FirstOrDefaultAsync(p => p.Email == employeeNumber, cancellationToken);

    public async Task<IEnumerable<Professor>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        => await context.Professors
            .Include(p => p.Subjects)
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Professors.CountAsync(cancellationToken);

    public async Task<Professor> CreateAsync(Professor professor, CancellationToken cancellationToken = default)
    {
        await context.Professors.AddAsync(professor, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return professor;
    }

    public async Task UpdateAsync(Professor professor, CancellationToken cancellationToken = default)
    {
        // Asegurar que EF esté rastreando la entidad
        context.Entry(professor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var professor = await context.Professors.FindAsync([id], cancellationToken);
        if (professor != null)
        {
            context.Professors.Remove(professor);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken = default)
        => await context.Professors.AnyAsync(p => p.Email == employeeNumber, cancellationToken);

    public async Task<bool> EmployeeNumberExistsExcludingProfessorAsync(string employeeNumber, int professorId, CancellationToken cancellationToken = default)
        => await context.Professors.AnyAsync(p => p.Email == employeeNumber && p.Id != professorId, cancellationToken);

    public async Task<int> GetSubjectCountAsync(int professorId, CancellationToken cancellationToken = default)
        => await context.Subjects.CountAsync(s => s.ProfessorId == professorId, cancellationToken);

    public async Task<IEnumerable<ProfessorWithWorkload>> GetProfessorsWithWorkloadAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        
        command.CommandText = @"
            SELECT id, name, specialization, email, phone, is_active, total_subjects, max_allowed, status 
            FROM view_professors 
            ORDER BY name 
            LIMIT @pageSize OFFSET @offset";
        
        var pageParam = command.CreateParameter();
        pageParam.ParameterName = "@pageSize";
        pageParam.Value = pageSize;
        command.Parameters.Add(pageParam);
        
        var offsetParam = command.CreateParameter();
        offsetParam.ParameterName = "@offset";
        offsetParam.Value = (page - 1) * pageSize;
        command.Parameters.Add(offsetParam);
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var result = new List<ProfessorWithWorkload>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new ProfessorWithWorkload(
                Id: reader.GetInt32(reader.GetOrdinal("id")),
                Name: reader.GetString(reader.GetOrdinal("name")),
                Specialization: reader.IsDBNull(reader.GetOrdinal("specialization")) ? null : reader.GetString(reader.GetOrdinal("specialization")),
                Email: reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                Phone: reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                IsActive: reader.GetBoolean(reader.GetOrdinal("is_active")),
                TotalSubjects: Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("total_subjects"))),
                MaxAllowed: reader.GetInt32(reader.GetOrdinal("max_allowed")),
                Status: reader.GetString(reader.GetOrdinal("status"))
            ));
        }
        
        return result;
    }
}
