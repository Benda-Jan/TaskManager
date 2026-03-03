using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        var isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        project.Update(request.Name, request.Description, request.Budget);
        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
