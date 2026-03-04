using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.RemoveMember;

public sealed record RemoveMemberCommand(Guid ProjectId, Guid TargetUserId) : IRequest;
