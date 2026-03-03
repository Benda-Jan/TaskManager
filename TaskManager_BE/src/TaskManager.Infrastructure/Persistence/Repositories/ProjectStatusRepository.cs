using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class ProjectStatusRepository(AppDbContext context) : IProjectStatusRepository
{
    public Task<ProjectStatus?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(ProjectStatus status, CancellationToken cancellationToken = default)
        => await context.ProjectStatuses.AddAsync(status, cancellationToken);

    public void Update(ProjectStatus status)
        => context.ProjectStatuses.Update(status);

    public void Remove(ProjectStatus status)
        => context.ProjectStatuses.Remove(status);
}
