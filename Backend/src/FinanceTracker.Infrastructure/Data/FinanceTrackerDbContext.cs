using FinanceTracker.DB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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

    /// <summary>
    /// Gets or sets the DbSet for User entities
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Account entities (financial accounts)
    /// </summary>
    public DbSet<Account> Accounts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Category entities
    /// </summary>
    public DbSet<Category> Categories { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for Transaction entities
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; } = null!;

    public override int SaveChanges()
    {
        SetTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetTimestamps()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is User user && entry.State == EntityState.Added)
            {
                // Only set if not already set (to allow explicit timestamp specification)
                if (user.CreatedAt == default)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Account account && entry.State == EntityState.Added)
            {
                // Only set if not already set (to allow explicit timestamp specification)
                if (account.CreatedAt == default)
                {
                    account.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Transaction transaction && entry.State == EntityState.Added)
            {
                // Only set if not already set (to allow explicit timestamp specification)
                if (transaction.Timestamp == default)
                {
                    transaction.Timestamp = DateTime.UtcNow;
                }
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure Account entity (financial account)
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AccountType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // Configure relationship with User
            entity.HasOne(e => e.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserId).IsRequired();

            // Configure relationship with User
            entity.HasOne(e => e.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);

            // Configure relationships
            entity.HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data for testing
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds initial data for testing purposes
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Create admin user for testing
        var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var adminPasswordHash = HashPassword("admin");

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminUserId,
            Email = "admin@gmail.com",
            PasswordHash = adminPasswordHash,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }

    /// <summary>
    /// Hashes a password using SHA256
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <returns>The hashed password</returns>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
