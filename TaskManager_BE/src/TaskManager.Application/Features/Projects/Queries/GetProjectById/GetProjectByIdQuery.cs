using MediatR;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDetailDto>;
