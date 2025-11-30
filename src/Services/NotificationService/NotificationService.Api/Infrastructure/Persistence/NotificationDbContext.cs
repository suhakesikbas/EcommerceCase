using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Domain.Entities;

namespace NotificationService.Api.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationMessage> NotificationMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<NotificationMessage>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(e => e.Id);
            entity.Property(x => x.Channel).IsRequired();
            entity.Property(x => x.Recipient).IsRequired();
            entity.Property(x => x.Subject).IsRequired();
        });
    }
}
