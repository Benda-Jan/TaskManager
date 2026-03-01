namespace TaskManager.Domain.Entities;

public sealed class ProjectMember
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public Guid InvitedByUserId { get; private set; }
    public Project Project { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private ProjectMember() { }

    public ProjectMember(Guid projectId, Guid userId, Guid invitedByUserId)
    {
        ProjectId = projectId;
        UserId = userId;
        InvitedByUserId = invitedByUserId;
        JoinedAt = DateTime.UtcNow;
    }
}
