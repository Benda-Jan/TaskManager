using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
{
    public async Task<ProjectDetailDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.Id, cancellationToken);

        if (project is null || !project.Members.Any(m => m.UserId == currentUser.UserId))
            throw new NotFoundException(nameof(Project), request.Id);

        return new ProjectDetailDto(
            project.Id,
            project.Name,
            project.Description,
            project.Budget,
            project.Currency,
            project.Members.Count,
            project.CreatedAt,
            project.Statuses
                .OrderBy(s => s.Order)
                .Select(s => new StatusDto(s.Id, s.Name, s.Color, s.Order))
                .ToList());
    }
}
