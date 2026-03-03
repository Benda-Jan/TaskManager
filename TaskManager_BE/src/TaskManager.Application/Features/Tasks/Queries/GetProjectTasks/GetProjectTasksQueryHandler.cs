using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Tasks.Queries.GetProjectTasks;

public sealed class GetProjectTasksQueryHandler(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetProjectTasksQuery, IReadOnlyList<TaskDto>>
{
    public async Task<IReadOnlyList<TaskDto>> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var tasks = await taskRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        return tasks
            .Select(t => new TaskDto(
                t.Id,
                t.ProjectId,
                t.ParentTaskId,
                t.Title,
                t.Description,
                t.Type.ToString(),
                t.StatusId,
                t.StartDate,
                t.EndDate,
                t.CreatedAt,
                t.Assignees.Select(a => new TaskAssigneeDto(a.TaskId, a.UserId, a.AssignedAt)).ToList()))
            .ToList();
    }
}
