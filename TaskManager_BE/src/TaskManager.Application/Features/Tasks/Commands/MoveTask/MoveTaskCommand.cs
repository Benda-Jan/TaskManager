using MediatR;

namespace TaskManager.Application.Features.Tasks.Commands.MoveTask;

public sealed record MoveTaskCommand(Guid TaskId, Guid StatusId) : IRequest;
