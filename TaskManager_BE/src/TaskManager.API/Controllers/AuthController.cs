using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Auth;
using TaskManager.Application.Features.Auth.Commands.Login;
using TaskManager.Application.Features.Auth.Commands.RefreshToken;
using TaskManager.Application.Features.Auth.Commands.Register;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthDto>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new RegisterCommand(request.Email, request.Name, request.Password), ct);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthDto>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AuthDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthDto>> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new RefreshTokenCommand(request.Token), ct);
        return Ok(result);
    }
}

public sealed record RegisterRequest(string Email, string Name, string Password);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshRequest(string Token);
