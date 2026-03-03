using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(AppDbContext context) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Tasks
            .Include(t => t.Assignees)
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await context.Tasks
            .Include(t => t.Assignees)
            .Where(t => t.ProjectId == projectId && t.ParentTaskId == null)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        => await context.Tasks.AddAsync(task, cancellationToken);

    public void Update(TaskItem task)
        => context.Tasks.Update(task);

    public void Remove(TaskItem task)
        => context.Tasks.Remove(task);

    public Task<bool> HasTasksForStatusAsync(Guid statusId, CancellationToken cancellationToken = default)
        => context.Tasks.AnyAsync(t => t.StatusId == statusId, cancellationToken);
}
