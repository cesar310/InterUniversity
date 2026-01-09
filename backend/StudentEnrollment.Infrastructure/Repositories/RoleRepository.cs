using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Domain.Entities;
using StudentEnrollment.Domain.Interfaces;
using StudentEnrollment.Infrastructure.Data;

namespace StudentEnrollment.Infrastructure.Repositories;

public sealed class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await context.Roles
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public async Task<IEnumerable<Role>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        => await context.Roles
            .Where(r => names.Contains(r.Name))
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Roles
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
}
