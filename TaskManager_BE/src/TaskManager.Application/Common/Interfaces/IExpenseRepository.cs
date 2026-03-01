using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Expense>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
    void Update(Expense expense);
    void Remove(Expense expense);
}
