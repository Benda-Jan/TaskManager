using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class ProjectInvitationRepository(AppDbContext context) : IProjectInvitationRepository
{
    public Task<ProjectInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => context.ProjectInvitations
            .FirstOrDefaultAsync(i => i.Token == token, cancellationToken);

    public Task<ProjectInvitation?> GetPendingByEmailAndProjectAsync(string email, Guid projectId, CancellationToken cancellationToken = default)
        => context.ProjectInvitations
            .FirstOrDefaultAsync(
                i => i.Email == email && i.ProjectId == projectId && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

    public async Task AddAsync(ProjectInvitation invitation, CancellationToken cancellationToken = default)
        => await context.ProjectInvitations.AddAsync(invitation, cancellationToken);
}
