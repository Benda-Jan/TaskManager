using MediatR;

namespace TaskManager.Application.Features.Expenses.Commands.CreateExpense;

public sealed record CreateExpenseCommand(
    Guid ProjectId,
    Guid? CategoryId,
    decimal Amount,
    string? Description,
    DateTime Date) : IRequest<Guid>;
