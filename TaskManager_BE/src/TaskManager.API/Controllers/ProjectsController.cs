using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Projects.Commands.CreateProject;
using TaskManager.Application.Features.Projects.Commands.CreateStatus;
using TaskManager.Application.Features.Projects.Commands.DeleteStatus;
using TaskManager.Application.Features.Projects.Commands.InviteMember;
using TaskManager.Application.Features.Projects.Commands.RemoveMember;
using TaskManager.Application.Features.Projects.Commands.UpdateProject;
using TaskManager.Application.Features.Projects.Commands.UpdateStatus;
using TaskManager.Application.Features.Projects.Queries.GetMyProjects;
using TaskManager.Application.Features.Projects.Queries.GetProjectById;
using TaskManager.Application.Features.Projects.Queries.GetProjectMembers;
using TaskManager.Application.Features.Expenses.Queries.GetProjectCategories;
using TaskManager.Application.Features.Expenses.Queries.GetProjectExpenses;
using TaskManager.Application.Features.Tasks.Queries.GetProjectTasks;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetMyProjects(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyProjectsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProjectDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateProject(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateProjectCommand(id, request.Name, request.Description, request.Budget), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/statuses")]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateStatus(Guid id, [FromBody] StatusRequest request, CancellationToken cancellationToken)
    {
        var statusId = await sender.Send(new CreateStatusCommand(id, request.Name, request.Color), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id = statusId });
    }

    [HttpPut("{id:guid}/statuses/{statusId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, Guid statusId, [FromBody] StatusRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateStatusCommand(statusId, request.Name, request.Color), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/statuses/{statusId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteStatus(Guid id, Guid statusId, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteStatusCommand(statusId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/tasks")]
    [ProducesResponseType<IReadOnlyList<TaskDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetTasks(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectTasksQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/expenses")]
    [ProducesResponseType<IReadOnlyList<ExpenseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ExpenseDto>>> GetExpenses(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectExpensesQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/expense-categories")]
    [ProducesResponseType<IReadOnlyList<CategoryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetExpenseCategories(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectCategoriesQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/members")]
    [ProducesResponseType<IReadOnlyList<MemberDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembers(Guid id, CancellationToken cancellationToken)
    {
        IReadOnlyList<MemberDto> result = await sender.Send(new GetProjectMembersQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/invitations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> InviteMember(Guid id, [FromBody] InviteMemberRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new InviteMemberCommand(id, request.Email), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveMemberCommand(id, userId), cancellationToken);
        return NoContent();
    }
}

public sealed record UpdateProjectRequest(string Name, string? Description, decimal Budget);
public sealed record StatusRequest(string Name, string Color);
public sealed record InviteMemberRequest(string Email);
