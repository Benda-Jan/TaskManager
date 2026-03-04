using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(User User, ProjectMember Membership)>> GetMembersAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    void Update(Project project);
    void Remove(Project project);
}
