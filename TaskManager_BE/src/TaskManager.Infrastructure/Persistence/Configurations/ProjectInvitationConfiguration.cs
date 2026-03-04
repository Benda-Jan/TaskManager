using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class ProjectInvitationConfiguration : IEntityTypeConfiguration<ProjectInvitation>
{
    public void Configure(EntityTypeBuilder<ProjectInvitation> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(i => i.Token)
            .IsUnique();

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasOne(i => i.Project)
            .WithMany(p => p.Invitations)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
