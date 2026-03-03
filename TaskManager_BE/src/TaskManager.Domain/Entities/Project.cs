namespace TaskManager.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Budget { get; private set; }
    public string Currency { get; private set; } = "CZK";
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<ProjectMember> Members { get; private set; } = [];
    public ICollection<ProjectStatus> Statuses { get; private set; } = [];
    public ICollection<TaskItem> Tasks { get; private set; } = [];
    public ICollection<Expense> Expenses { get; private set; } = [];
    public ICollection<ExpenseCategory> ExpenseCategories { get; private set; } = [];

    private Project() { }

    public Project(string name, string? description, decimal budget, Guid createdByUserId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Budget = budget;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        Currency = "CZK";
    }

    public void Update(string name, string? description, decimal budget)
    {
        Name = name;
        Description = description;
        Budget = budget;
    }

    public void AddMember(Guid userId, Guid invitedByUserId)
    {
        Members.Add(new ProjectMember(Id, userId, invitedByUserId));
    }

    public void AddStatus(string name, string color, int order)
    {
        Statuses.Add(new ProjectStatus(Id, name, color, order));
    }
}
