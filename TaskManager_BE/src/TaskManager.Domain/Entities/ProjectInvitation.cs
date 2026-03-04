namespace TaskManager.Domain.Entities;

public sealed class ProjectInvitation
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Token { get; private set; } = string.Empty;
    public Guid InvitedByUserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public Project Project { get; private set; } = null!;

    private ProjectInvitation() { }

    public ProjectInvitation(Guid projectId, string email, string token, Guid invitedByUserId)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Email = email.ToLowerInvariant();
        Token = token;
        InvitedByUserId = invitedByUserId;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
    }

    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;

    public void MarkUsed() => IsUsed = true;
}
