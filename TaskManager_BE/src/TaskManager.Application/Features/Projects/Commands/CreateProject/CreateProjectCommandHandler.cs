using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser
) : IRequestHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var project = new Project(request.Name, request.Description, request.Budget, userId);

        project.AddMember(userId, userId);

        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
