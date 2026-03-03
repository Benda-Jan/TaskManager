using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Commands.UpdateStatus;

public sealed class UpdateStatusCommandHandler(
    IProjectStatusRepository statusRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateStatusCommand>
{
    public async Task Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var status = await statusRepository.GetByIdAsync(request.StatusId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectStatus), request.StatusId);

        var isMember = await projectRepository.IsMemberAsync(status.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(ProjectStatus), request.StatusId);

        status.Update(request.Name, request.Color, status.Order);
        statusRepository.Update(status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
