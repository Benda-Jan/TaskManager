namespace TaskManager.Application.Features.Projects.Queries.GetProjectById;

public sealed record StatusDto(Guid Id, string Name, string Color, int Order);

public sealed record ProjectDetailDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Budget,
    string Currency,
    int MemberCount,
    DateTime CreatedAt,
    IReadOnlyList<StatusDto> Statuses);
