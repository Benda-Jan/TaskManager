using MediatR;

namespace TaskManager.Application.Features.Expenses.Queries.GetProjectExpenses;

public sealed record GetProjectExpensesQuery(Guid ProjectId) : IRequest<IReadOnlyList<ExpenseDto>>;
