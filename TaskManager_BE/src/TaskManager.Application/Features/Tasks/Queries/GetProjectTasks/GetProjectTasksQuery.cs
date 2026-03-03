using MediatR;

namespace TaskManager.Application.Features.Tasks.Queries.GetProjectTasks;

public sealed record GetProjectTasksQuery(Guid ProjectId) : IRequest<IReadOnlyList<TaskDto>>;
