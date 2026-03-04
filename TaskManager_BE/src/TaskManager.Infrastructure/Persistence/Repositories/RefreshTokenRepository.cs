using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        => await context.RefreshTokens.AddAsync(token, cancellationToken);

    public void Remove(RefreshToken token)
        => context.RefreshTokens.Remove(token);
}
