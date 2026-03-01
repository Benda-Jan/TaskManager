using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class TaskAssigneeConfiguration : IEntityTypeConfiguration<TaskAssignee>
{
    public void Configure(EntityTypeBuilder<TaskAssignee> builder)
    {
        builder.HasKey(a => new { a.TaskId, a.UserId });

        builder.HasOne(a => a.Task)
            .WithMany(t => t.Assignees)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
