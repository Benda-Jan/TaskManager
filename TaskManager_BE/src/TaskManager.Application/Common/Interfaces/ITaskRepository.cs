using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    void Update(TaskItem task);
    void Remove(TaskItem task);
}
