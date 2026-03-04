using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.InviteMember;

public sealed record InviteMemberCommand(Guid ProjectId, string Email) : IRequest;
