namespace TaskManager.Application.Features.Tasks.Queries.GetProjectTasks;

public sealed record TaskAssigneeDto(Guid TaskId, Guid UserId, DateTime AssignedAt);

public sealed record TaskDto(
    Guid Id,
    Guid ProjectId,
    Guid? ParentTaskId,
    string Title,
    string? Description,
    string Type,
    Guid StatusId,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime CreatedAt,
    IReadOnlyList<TaskAssigneeDto> Assignees);
