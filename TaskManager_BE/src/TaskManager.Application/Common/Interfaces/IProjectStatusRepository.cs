using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IProjectStatusRepository
{
    Task<ProjectStatus?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectStatus status, CancellationToken cancellationToken = default);
    void Update(ProjectStatus status);
    void Remove(ProjectStatus status);
}
