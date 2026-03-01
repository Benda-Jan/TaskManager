using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Projects.Commands.CreateProject;
using TaskManager.Application.Features.Projects.Queries.GetMyProjects;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetMyProjects(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyProjectsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProject(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMyProjects), new { }, new { id });
    }
}
