namespace TaskManager.Application.Features.Projects.Queries.GetProjectMembers;

public sealed record MemberDto(Guid UserId, string Name, string Email, DateTime JoinedAt, bool IsCreator);
