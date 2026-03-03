using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class ExpenseCategoryRepository(AppDbContext context) : IExpenseCategoryRepository
{
    public Task<ExpenseCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.ExpenseCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ExpenseCategory>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await context.ExpenseCategories
            .Where(c => c.ProjectId == projectId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ExpenseCategory category, CancellationToken cancellationToken = default)
        => await context.ExpenseCategories.AddAsync(category, cancellationToken);

    public void Remove(ExpenseCategory category)
        => context.ExpenseCategories.Remove(category);
}
