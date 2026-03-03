using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.CreateStatus;

public sealed record CreateStatusCommand(Guid ProjectId, string Name, string Color) : IRequest<Guid>;
