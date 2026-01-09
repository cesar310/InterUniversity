using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class DashboardRepository(ApplicationDbContext context) : IDashboardRepository
{
    public async Task<DashboardStats> GetSystemStatisticsAsync(CancellationToken cancellationToken = default)
    {
        // Ejecutar el stored procedure get_system_statistics()
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        
        command.CommandText = "CALL get_system_statistics()";
        
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        if (await reader.ReadAsync(cancellationToken))
        {
            return new DashboardStats(
                TotalStudents: reader.GetInt32(reader.GetOrdinal("total_students")),
                TotalProfessors: reader.GetInt32(reader.GetOrdinal("total_professors")),
                ActiveSubjects: reader.GetInt32(reader.GetOrdinal("active_subjects")),
                ActiveEnrollments: reader.GetInt32(reader.GetOrdinal("active_enrollments")),
                AvgSubjectsPerStudent: reader.IsDBNull(reader.GetOrdinal("avg_subjects_per_student")) 
                    ? 0m 
                    : reader.GetDecimal(reader.GetOrdinal("avg_subjects_per_student")),
                MaxSubjectsEnrolled: reader.IsDBNull(reader.GetOrdinal("max_subjects_enrolled")) 
                    ? 0 
                    : reader.GetInt32(reader.GetOrdinal("max_subjects_enrolled"))
            );
        }

        // Si no hay resultados, retornar valores por defecto
        return new DashboardStats(0, 0, 0, 0, 0m, 0);
    }
}
