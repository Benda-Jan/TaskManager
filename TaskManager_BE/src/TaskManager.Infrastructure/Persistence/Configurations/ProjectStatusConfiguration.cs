using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class ProjectStatusConfiguration : IEntityTypeConfiguration<ProjectStatus>
{
    public void Configure(EntityTypeBuilder<ProjectStatus> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Color).IsRequired().HasMaxLength(7);

        builder.HasOne(s => s.Project)
            .WithMany(p => p.Statuses)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
