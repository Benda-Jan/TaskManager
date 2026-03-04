using MediatR;

namespace TaskManager.Application.Features.Projects.Commands.AcceptInvitation;

public sealed record AcceptInvitationCommand(string Token, Guid UserId, string UserEmail) : IRequest;
