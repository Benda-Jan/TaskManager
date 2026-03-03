using MediatR;

namespace TaskManager.Application.Features.Expenses.Commands.CreateExpenseCategory;

public sealed record CreateExpenseCategoryCommand(Guid ProjectId, string Name) : IRequest<Guid>;
