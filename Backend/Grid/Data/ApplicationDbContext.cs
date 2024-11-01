using Grid.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Grid.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<BitcoinDetailModel> BitcoinDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BitcoinDetailModel>(entity =>
        {
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)");
        });
    }
}
