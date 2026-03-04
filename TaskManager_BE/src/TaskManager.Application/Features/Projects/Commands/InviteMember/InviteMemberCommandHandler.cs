using System.Security.Cryptography;
using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Commands.InviteMember;

public sealed class InviteMemberCommandHandler(
    IProjectRepository projectRepository,
    IProjectInvitationRepository invitationRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork,
    IAppSettings appSettings)
    : IRequestHandler<InviteMemberCommand>
{
    public async Task Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        bool isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException("Project", request.ProjectId);

        Project? project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            throw new NotFoundException("Project", request.ProjectId);

        string normalizedEmail = request.Email.ToLowerInvariant();

        User? invitee = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (invitee is not null)
        {
            bool alreadyMember = await projectRepository.IsMemberAsync(request.ProjectId, invitee.Id, cancellationToken);
            if (alreadyMember)
                throw new ConflictException("This user is already a member of the project.");
        }

        ProjectInvitation? existingInvitation = await invitationRepository.GetPendingByEmailAndProjectAsync(normalizedEmail, request.ProjectId, cancellationToken);
        if (existingInvitation is not null)
            return;

        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        ProjectInvitation invitation = new(request.ProjectId, normalizedEmail, token, currentUser.UserId);
        await invitationRepository.AddAsync(invitation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        User? inviter = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);
        string inviterName = inviter?.Name ?? "A team member";
        string inviteUrl = $"{appSettings.AppUrl}/login?invite={Uri.EscapeDataString(token)}";

        await emailService.SendInvitationAsync(normalizedEmail, inviterName, project.Name, inviteUrl, cancellationToken);
    }
}
