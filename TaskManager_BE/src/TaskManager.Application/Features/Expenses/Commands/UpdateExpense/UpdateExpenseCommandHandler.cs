using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Application.Features.Expenses.Commands.UpdateExpense;

public sealed class UpdateExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateExpenseCommand>
{
    public async Task Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetByIdAsync(request.ExpenseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Expense), request.ExpenseId);

        var isMember = await projectRepository.IsMemberAsync(expense.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember)
            throw new NotFoundException(nameof(Expense), request.ExpenseId);

        expense.Update(request.CategoryId, request.Amount, request.Description, request.Date);

        expenseRepository.Update(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
