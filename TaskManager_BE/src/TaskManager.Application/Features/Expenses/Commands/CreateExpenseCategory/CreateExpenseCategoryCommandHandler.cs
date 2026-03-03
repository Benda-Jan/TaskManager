using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Expenses.Commands.CreateExpenseCategory;

public sealed class CreateExpenseCategoryCommandHandler(
    IExpenseCategoryRepository categoryRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateExpenseCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var category = new ExpenseCategory(request.ProjectId, request.Name);
        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
