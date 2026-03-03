using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Expenses.Queries.GetProjectExpenses;

public sealed class GetProjectExpensesQueryHandler(
    IExpenseRepository expenseRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetProjectExpensesQuery, IReadOnlyList<ExpenseDto>>
{
    public async Task<IReadOnlyList<ExpenseDto>> Handle(GetProjectExpensesQuery request, CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var expenses = await expenseRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        return expenses
            .Select(e => new ExpenseDto(
                e.Id,
                e.ProjectId,
                e.CategoryId,
                e.Amount,
                e.Description,
                e.Date,
                e.CreatedByUserId,
                e.CreatedAt,
                e.Category is null ? null : new ExpenseCategoryDto(e.Category.Id, e.Category.Name)))
            .ToList();
    }
}
