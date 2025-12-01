using EcommerceApp.Shared.Domain;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceApp.Shared.Infrastructure.EntityFramework;

public class QueryRepository<TContext, TEntity, TPrimaryKey> : RepositoryBase<TContext>, IQueryRepository<TContext, TEntity, TPrimaryKey>, IRepositoryBase<TContext> where TContext : DbContext where TEntity : class, IEntityBase<TPrimaryKey>
{
    public DbSet<TEntity> DbSet { get; }

    public QueryRepository(TContext context)
        : base(context)
    {
        DbSet = base.Context.Set<TEntity>();
    }

    public IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> queryable = DbSet.AsQueryable();
        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return queryable;
    }

    public List<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable(predicate).ToList();
    }

    public Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable(predicate).ToListAsync();
    }

    public TEntity? GetById(TPrimaryKey id)
    {
        return DbSet.Find(id);
    }

    public async Task<TEntity?> GetByIdAsync(TPrimaryKey id)
    {
        return await DbSet.FindAsync(id);
    }

    public List<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod) where T : class
    {
        T val = queryMethod(Queryable());
        if (val is IEnumerable<T> source)
        {
            return source.ToList();
        }

        return new List<T> { val };
    }
}