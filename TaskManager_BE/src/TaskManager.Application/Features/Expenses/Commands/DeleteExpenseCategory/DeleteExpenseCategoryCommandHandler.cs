using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Expenses.Commands.DeleteExpenseCategory;

public sealed class DeleteExpenseCategoryCommandHandler(
    IExpenseCategoryRepository categoryRepository,
    IExpenseRepository expenseRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteExpenseCategoryCommand>
{
    public async Task Handle(DeleteExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExpenseCategory), request.CategoryId);

        var isMember = await projectRepository.IsMemberAsync(category.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(ExpenseCategory), request.CategoryId);

        var hasExpenses = await expenseRepository.HasExpensesForCategoryAsync(request.CategoryId, cancellationToken);
        if (hasExpenses)
            throw new ConflictException($"Group '{category.Name}' has expenses assigned and cannot be deleted.");

        categoryRepository.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
