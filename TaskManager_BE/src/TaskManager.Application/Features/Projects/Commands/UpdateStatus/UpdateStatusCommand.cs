using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.UpdateStatus;

public sealed record UpdateStatusCommand(Guid StatusId, string Name, string Color) : IRequest;
