using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public AppDbContext() { }

    public virtual DbSet<SearchHistory> SearchHistory { get; set; }

    public DbSet<UserSearch> UserSearches { get; set; }
}
