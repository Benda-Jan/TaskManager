using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Tasks.Commands.MoveTask;

public sealed class MoveTaskCommandHandler(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<MoveTaskCommand>
{
    public async Task Handle(MoveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskId);

        var isMember = await projectRepository.IsMemberAsync(task.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(TaskItem), request.TaskId);

        task.Update(task.Title, task.Description, request.StatusId, task.StartDate, task.EndDate);

        taskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
