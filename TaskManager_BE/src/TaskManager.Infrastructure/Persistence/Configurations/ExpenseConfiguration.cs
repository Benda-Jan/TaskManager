using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000);

        builder.HasOne(e => e.Project)
            .WithMany(p => p.Expenses)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
