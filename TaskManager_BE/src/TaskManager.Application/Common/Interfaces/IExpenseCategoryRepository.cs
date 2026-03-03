using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IExpenseCategoryRepository
{
    Task<ExpenseCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseCategory>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(ExpenseCategory category, CancellationToken cancellationToken = default);
    void Remove(ExpenseCategory category);
}
