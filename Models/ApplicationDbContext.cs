using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Transaction> Transactions { get; set; }

    //public DbSet<Category> Categories { get; set; }
}
