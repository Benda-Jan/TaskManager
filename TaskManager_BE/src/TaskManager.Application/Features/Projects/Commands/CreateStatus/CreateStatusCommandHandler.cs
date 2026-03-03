using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Commands.CreateStatus;

public sealed class CreateStatusCommandHandler(
    IProjectRepository projectRepository,
    IProjectStatusRepository statusRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateStatusCommand, Guid>
{
    public async Task<Guid> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        var isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        int nextOrder = project.Statuses.Count > 0 ? project.Statuses.Max(s => s.Order) + 1 : 0;
        var status = new ProjectStatus(request.ProjectId, request.Name, request.Color, nextOrder);
        await statusRepository.AddAsync(status, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return status.Id;
    }
}
