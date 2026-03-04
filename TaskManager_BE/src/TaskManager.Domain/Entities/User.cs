namespace TaskManager.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public ICollection<ProjectMember> ProjectMemberships { get; private set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

    private User() { }

    public User(string email, string name)
    {
        Id = Guid.NewGuid();
        Email = email;
        Name = name;
    }

    public void SetPasswordHash(string hash) => PasswordHash = hash;

    public void UpdateProfile(string email, string name)
    {
        Email = email;
        Name = name;
    }
}
