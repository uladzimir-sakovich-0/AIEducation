using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Data;

/// <summary>
/// Database context for the FinanceTracker application
/// </summary>
public class FinanceTrackerDbContext : DbContext
{
    public FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entity configurations will be added here
    }
}
