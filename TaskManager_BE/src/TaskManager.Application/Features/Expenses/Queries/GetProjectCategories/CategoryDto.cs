namespace TaskManager.Application.Features.Expenses.Queries.GetProjectCategories;

public sealed record CategoryDto(Guid Id, Guid ProjectId, string Name);
