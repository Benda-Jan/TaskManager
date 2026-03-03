using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Expenses.Commands.DeleteExpense;

public sealed class DeleteExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteExpenseCommand>
{
    public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetByIdAsync(request.ExpenseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Expense), request.ExpenseId);

        var isMember = await projectRepository.IsMemberAsync(expense.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Expense), request.ExpenseId);

        expenseRepository.Remove(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
