using MediatR;
using TaskManager.Application.Features.Auth;

namespace TaskManager.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Name, string Password) : IRequest<AuthDto>;
