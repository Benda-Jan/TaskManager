using MediatR;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Application.Features.Projects.Queries.GetMyProjects;

public sealed class GetMyProjectsQueryHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetMyProjectsQuery, IReadOnlyList<ProjectDto>>
{
    public async Task<IReadOnlyList<ProjectDto>> Handle(
        GetMyProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetByUserIdAsync(currentUser.UserId, cancellationToken);

        return projects
            .Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.Budget,
                p.Currency,
                p.Members.Count,
                p.CreatedAt))
            .ToList();
    }
}
