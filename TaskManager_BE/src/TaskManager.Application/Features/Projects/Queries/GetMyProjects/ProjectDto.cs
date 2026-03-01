namespace TaskManager.Application.Features.Projects.Queries.GetMyProjects;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Budget,
    string Currency,
    int MemberCount,
    DateTime CreatedAt
);
