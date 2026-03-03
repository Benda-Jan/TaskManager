using MediatR;

namespace TaskManager.Application.Features.Expenses.Commands.UpdateExpense;

public sealed record UpdateExpenseCommand(
    Guid ExpenseId,
    Guid? CategoryId,
    decimal Amount,
    string? Description,
    DateTime Date) : IRequest;
