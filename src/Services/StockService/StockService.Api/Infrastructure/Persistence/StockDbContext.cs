using Microsoft.EntityFrameworkCore;

namespace StockService.Api.Infrastructure.Persistence;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
    {
    }
    public DbSet<Domain.Entities.ProductInventory> ProductInventories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.ProductInventory>(entity =>
        {
            entity.ToTable("product_inventory");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProductId).IsRequired();
            entity.Property(x => x.AvailableQuantity).IsRequired();
        });
    }
}
