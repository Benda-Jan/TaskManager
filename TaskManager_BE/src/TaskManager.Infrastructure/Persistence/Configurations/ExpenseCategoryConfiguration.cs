using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(c => c.Project)
            .WithMany(p => p.ExpenseCategories)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
