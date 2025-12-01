using EcommerceApp.Shared.Domain;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Shared.Infrastructure.EntityFramework;

public class CommandRepository<TContext, TEntity, TPrimaryKey> : RepositoryBase<TContext>, ICommandRepository<TContext, TEntity, TPrimaryKey>, IRepositoryBase<TContext> where TContext : DbContext where TEntity : class, IEntityBase<TPrimaryKey>
{
    public DbSet<TEntity> DbSet { get; }

    public CommandRepository(TContext context)
        : base(context)
    {
        DbSet = base.Context.Set<TEntity>();
    }

    public void AttachIfNot(TEntity entity)
    {
        if (!DbSet.Local.Contains(entity))
        {
            DbSet.Attach(entity);
        }
    }

    public TEntity Insert(TEntity entity)
    {
        return InsertAsync(entity).GetAwaiter().GetResult();
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        return (await DbSet.AddAsync(entity, cancellationToken)).Entity;
    }

    public List<TEntity> InsertMany(IEnumerable<TEntity> entities)
    {
        return InsertManyAsync(entities).GetAwaiter().GetResult();
    }

    public async Task<List<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        TEntity[] aggregateBases = (entities as TEntity[]) ?? entities.ToArray();
        if (!aggregateBases.Any())
        {
            return aggregateBases.ToList();
        }

        await DbSet.AddRangeAsync(aggregateBases, cancellationToken);
        return aggregateBases.ToList();
    }

    public TEntity Update(TEntity entity)
    {
        AttachIfNot(entity);
        base.Context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public void Delete(TEntity entity)
    {
        AttachIfNot(entity);
        DbSet.Remove(entity);
    }

    public void Delete(TPrimaryKey id)
    {
        TEntity val = DbSet.Local.FirstOrDefault((TEntity c) => c.Id.Equals(id));
        if (val == null)
        {
            val = DbSet.Find(id);
            if (val == null)
            {
                return;
            }
        }

        Delete(val);
    }

    public void DeleteMany(IEnumerable<TEntity> entities)
    {
        TEntity[] array = (entities as TEntity[]) ?? entities.ToArray();
        if (!array.Any())
        {
            return;
        }

        TEntity[] array2 = array;
        foreach (TEntity entity in array2)
        {
            AttachIfNot(entity);
        }

        TEntity val = array.First();
        DbSet.RemoveRange(array);
    }
}