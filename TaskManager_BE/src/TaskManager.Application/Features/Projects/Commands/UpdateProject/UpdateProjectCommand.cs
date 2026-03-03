using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(Guid ProjectId, string Name, string? Description, decimal Budget) : IRequest;
