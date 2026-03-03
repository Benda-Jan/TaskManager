using MediatR;

namespace TaskManager.Application.Features.Expenses.Queries.GetProjectCategories;

public sealed record GetProjectCategoriesQuery(Guid ProjectId) : IRequest<IReadOnlyList<CategoryDto>>;
