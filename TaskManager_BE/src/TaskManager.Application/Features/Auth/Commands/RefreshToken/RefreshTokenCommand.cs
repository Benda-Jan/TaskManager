using MediatR;
using TaskManager.Application.Features.Auth;

namespace TaskManager.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token) : IRequest<AuthDto>;
