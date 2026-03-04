using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IProjectInvitationRepository
{
    Task<ProjectInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<ProjectInvitation?> GetPendingByEmailAndProjectAsync(string email, Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectInvitation invitation, CancellationToken cancellationToken = default);
}
