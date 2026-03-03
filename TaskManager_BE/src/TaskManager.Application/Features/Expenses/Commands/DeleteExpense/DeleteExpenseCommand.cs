using MediatR;

namespace TaskManager.Application.Features.Expenses.Commands.DeleteExpense;

public sealed record DeleteExpenseCommand(Guid ExpenseId) : IRequest;
