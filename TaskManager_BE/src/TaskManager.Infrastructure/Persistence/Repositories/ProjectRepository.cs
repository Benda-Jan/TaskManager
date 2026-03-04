using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(AppDbContext context) : IProjectRepository
{
    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Projects
            .Include(p => p.Members)
            .Include(p => p.Statuses.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.Projects
            .Where(p => p.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);

    public Task<bool> IsMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        => context.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<(User User, ProjectMember Membership)>> GetMembersAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        List<ProjectMember> members = await context.ProjectMembers
            .Where(m => m.ProjectId == projectId)
            .Include(m => m.User)
            .ToListAsync(cancellationToken);

        return members.Select(m => (m.User, m)).ToList();
    }

    public async Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        ProjectMember? member = await context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId, cancellationToken);
        if (member is not null)
            context.ProjectMembers.Remove(member);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
        => await context.Projects.AddAsync(project, cancellationToken);

    public void Update(Project project)
        => context.Projects.Update(project);

    public void Remove(Project project)
        => context.Projects.Remove(project);
}
