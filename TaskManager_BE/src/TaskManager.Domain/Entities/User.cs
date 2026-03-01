namespace TaskManager.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ICollection<ProjectMember> ProjectMemberships { get; private set; } = [];

    private User() { }

    public User(Guid id, string email, string name)
    {
        Id = id;
        Email = email;
        Name = name;
    }

    public void UpdateProfile(string email, string name)
    {
        Email = email;
        Name = name;
    }
}
