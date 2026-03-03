using MediatR;
using TaskManager.Application.Features.Auth;

namespace TaskManager.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthDto>;
