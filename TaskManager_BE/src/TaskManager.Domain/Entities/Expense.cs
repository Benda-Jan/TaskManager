namespace TaskManager.Domain.Entities;

public sealed class Expense
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }
    public DateTime Date { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Project Project { get; private set; } = null!;
    public ExpenseCategory? Category { get; private set; }

    private Expense() { }

    public Expense(Guid projectId, Guid? categoryId, decimal amount, string? description, DateTime date, Guid createdByUserId)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        CategoryId = categoryId;
        Amount = amount;
        Description = description;
        Date = date;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(Guid? categoryId, decimal amount, string? description, DateTime date)
    {
        CategoryId = categoryId;
        Amount = amount;
        Description = description;
        Date = date;
    }
}
