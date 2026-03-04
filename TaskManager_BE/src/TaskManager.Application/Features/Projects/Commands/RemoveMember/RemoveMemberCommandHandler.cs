using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Commands.RemoveMember;

public sealed class RemoveMemberCommandHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveMemberCommand>
{
    public async Task Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        bool isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException("Project", request.ProjectId);

        Project? project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            throw new NotFoundException("Project", request.ProjectId);

        if (project.CreatedByUserId == request.TargetUserId)
            throw new ConflictException("The project creator cannot be removed.");

        await projectRepository.RemoveMemberAsync(request.ProjectId, request.TargetUserId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
