using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Expenses.Commands.CreateExpense;
using TaskManager.Application.Features.Expenses.Commands.DeleteExpense;
using TaskManager.Application.Features.Expenses.Commands.UpdateExpense;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ExpensesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateExpense(
        [FromBody] CreateExpenseCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Created(string.Empty, new { id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpense(
        Guid id,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateExpenseCommand(id, request.CategoryId, request.Amount, request.Description, request.Date), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteExpenseCommand(id), cancellationToken);
        return NoContent();
    }
}

public sealed record UpdateExpenseRequest(Guid? CategoryId, decimal Amount, string? Description, DateTime Date);
