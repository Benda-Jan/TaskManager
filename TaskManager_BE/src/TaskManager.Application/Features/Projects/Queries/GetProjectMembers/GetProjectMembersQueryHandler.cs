using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectMembers;

public sealed class GetProjectMembersQueryHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetProjectMembersQuery, IReadOnlyList<MemberDto>>
{
    public async Task<IReadOnlyList<MemberDto>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
    {
        bool isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException("Project", request.ProjectId);

        Project? project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            throw new NotFoundException("Project", request.ProjectId);

        IReadOnlyList<(User User, ProjectMember Membership)> members =
            await projectRepository.GetMembersAsync(request.ProjectId, cancellationToken);

        return members
            .Select(m => new MemberDto(
                m.User.Id,
                m.User.Name,
                m.User.Email,
                m.Membership.JoinedAt,
                m.User.Id == project.CreatedByUserId))
            .ToList();
    }
}
