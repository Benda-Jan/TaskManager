using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    Guid StatusId,
    string Title,
    string? Description,
    TaskType Type) : IRequest<Guid>;
