using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Users.AnyAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await context.Users.AddAsync(user, cancellationToken);

    public void Update(User user)
        => context.Users.Update(user);
}
