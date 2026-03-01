namespace TaskManager.Domain.Entities;

public sealed class ProjectStatus
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = "#6B7280";
    public int Order { get; private set; }
    public Project Project { get; private set; } = null!;
    public ICollection<TaskItem> Tasks { get; private set; } = [];

    private ProjectStatus() { }

    public ProjectStatus(Guid projectId, string name, string color, int order)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Name = name;
        Color = color;
        Order = order;
    }

    public void Update(string name, string color, int order)
    {
        Name = name;
        Color = color;
        Order = order;
    }
}
