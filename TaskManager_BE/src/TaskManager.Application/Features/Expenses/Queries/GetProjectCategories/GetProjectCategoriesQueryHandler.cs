using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Expenses.Queries.GetProjectCategories;

public sealed class GetProjectCategoriesQueryHandler(
    IExpenseCategoryRepository categoryRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetProjectCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetProjectCategoriesQuery request, CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var categories = await categoryRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        return categories.Select(c => new CategoryDto(c.Id, c.ProjectId, c.Name)).ToList();
    }
}
