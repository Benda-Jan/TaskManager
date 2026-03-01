using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    // Keycloak puts the user ID in the "sub" claim
    public Guid UserId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirstValue("sub"), out var id)
            ? id
            : Guid.Empty;

    public string? Email => accessor.HttpContext?.User.FindFirstValue("email");

    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
