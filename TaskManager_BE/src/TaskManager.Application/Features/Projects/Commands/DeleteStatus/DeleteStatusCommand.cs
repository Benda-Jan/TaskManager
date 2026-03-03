using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.DeleteStatus;

public sealed record DeleteStatusCommand(Guid StatusId) : IRequest;
