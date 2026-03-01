using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string? Description,
    decimal Budget
) : IRequest<Guid>;
