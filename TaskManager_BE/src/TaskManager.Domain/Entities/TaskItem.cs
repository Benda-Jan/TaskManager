using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? ParentTaskId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskType Type { get; private set; }
    public Guid StatusId { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? RecurrencePattern { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Project Project { get; private set; } = null!;
    public TaskItem? ParentTask { get; private set; }
    public ProjectStatus Status { get; private set; } = null!;
    public ICollection<TaskItem> SubTasks { get; private set; } = [];
    public ICollection<TaskAssignee> Assignees { get; private set; } = [];

    private TaskItem() { }

    public TaskItem(
        Guid projectId,
        string title,
        string? description,
        TaskType type,
        Guid statusId,
        Guid createdByUserId,
        Guid? parentTaskId = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        ParentTaskId = parentTaskId;
        Title = title;
        Description = description;
        Type = type;
        StatusId = statusId;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Update(string title, string? description, Guid statusId, DateTime? startDate, DateTime? endDate)
    {
        Title = title;
        Description = description;
        StatusId = statusId;
        StartDate = startDate;
        EndDate = endDate;
    }
}
