namespace TaskManager.Domain.Entities;

public sealed class TaskAssignee
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public TaskItem Task { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TaskAssignee() { }

    public TaskAssignee(Guid taskId, Guid userId)
    {
        TaskId = taskId;
        UserId = userId;
        AssignedAt = DateTime.UtcNow;
    }
}
