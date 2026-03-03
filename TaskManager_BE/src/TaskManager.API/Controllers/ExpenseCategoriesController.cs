using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Expenses.Commands.CreateExpenseCategory;
using TaskManager.Application.Features.Expenses.Commands.DeleteExpenseCategory;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/expense-categories")]
[Authorize]
public sealed class ExpenseCategoriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateCategory(
        [FromBody] CreateExpenseCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created(string.Empty, new { id });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteExpenseCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
