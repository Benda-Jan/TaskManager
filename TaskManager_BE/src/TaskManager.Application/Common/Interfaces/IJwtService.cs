namespace TaskManager.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, string name);
    string GenerateRefreshToken();
    TimeSpan RefreshTokenExpiry { get; }
}
