using MediatR;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectMembers;

public sealed record GetProjectMembersQuery(Guid ProjectId) : IRequest<IReadOnlyList<MemberDto>>;
