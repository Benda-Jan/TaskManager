using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Commands.DeleteStatus;

public sealed class DeleteStatusCommandHandler(
    IProjectStatusRepository statusRepository,
    IProjectRepository projectRepository,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteStatusCommand>
{
    public async Task Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
    {
        var status = await statusRepository.GetByIdAsync(request.StatusId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectStatus), request.StatusId);

        var isMember = await projectRepository.IsMemberAsync(status.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(ProjectStatus), request.StatusId);

        var hasTasks = await taskRepository.HasTasksForStatusAsync(request.StatusId, cancellationToken);
        if (hasTasks)
            throw new ConflictException($"Status '{status.Name}' has tasks assigned and cannot be deleted.");

        statusRepository.Remove(status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
