namespace TaskManager.Domain.Entities;

public sealed class ExpenseCategory
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Project Project { get; private set; } = null!;
    public ICollection<Expense> Expenses { get; private set; } = [];

    private ExpenseCategory() { }

    public ExpenseCategory(Guid projectId, string name)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Name = name;
    }

    public void Rename(string name) => Name = name;
}
