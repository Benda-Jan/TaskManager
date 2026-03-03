namespace TaskManager.Application.Features.Expenses.Queries.GetProjectExpenses;

public sealed record ExpenseCategoryDto(Guid Id, string Name);

public sealed record ExpenseDto(
    Guid Id,
    Guid ProjectId,
    Guid? CategoryId,
    decimal Amount,
    string? Description,
    DateTime Date,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    ExpenseCategoryDto? Category);
