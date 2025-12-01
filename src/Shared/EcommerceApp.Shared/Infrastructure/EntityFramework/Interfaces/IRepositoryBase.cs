using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;

public interface IRepositoryBase<out TContext> where TContext : DbContext
{
    TContext Context { get; }

    int Save();

    Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
}