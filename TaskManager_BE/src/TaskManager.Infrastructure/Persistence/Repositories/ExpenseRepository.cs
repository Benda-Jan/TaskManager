using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public sealed class ExpenseRepository(AppDbContext context) : IExpenseRepository
{
    public Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Expense>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
        => await context.Expenses.AddAsync(expense, cancellationToken);

    public void Update(Expense expense)
        => context.Expenses.Update(expense);

    public void Remove(Expense expense)
        => context.Expenses.Remove(expense);
}
