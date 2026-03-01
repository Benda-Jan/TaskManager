using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController(ISender sender) : ControllerBase
{
    // TODO: implement endpoints
}
