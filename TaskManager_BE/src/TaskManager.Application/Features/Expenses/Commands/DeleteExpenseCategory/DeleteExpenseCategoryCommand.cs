using MediatR;

namespace TaskManager.Application.Features.Expenses.Commands.DeleteExpenseCategory;

public sealed record DeleteExpenseCategoryCommand(Guid CategoryId) : IRequest;
