using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Projects.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommandHandler(
    IProjectInvitationRepository invitationRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AcceptInvitationCommand>
{
    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        ProjectInvitation? invitation = await invitationRepository.GetByTokenAsync(request.Token, cancellationToken);
        if (invitation is null || !invitation.IsValid)
            return;

        if (!string.Equals(invitation.Email, request.UserEmail, StringComparison.OrdinalIgnoreCase))
            return;

        Project? project = await projectRepository.GetByIdAsync(invitation.ProjectId, cancellationToken);
        if (project is null)
            return;

        bool alreadyMember = await projectRepository.IsMemberAsync(invitation.ProjectId, request.UserId, cancellationToken);
        if (!alreadyMember)
            project.AddMember(request.UserId, invitation.InvitedByUserId);

        invitation.MarkUsed();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
