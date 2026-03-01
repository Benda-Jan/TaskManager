using MediatR;

namespace TaskManager.Application.Features.Projects.Queries.GetMyProjects;

public sealed record GetMyProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;
