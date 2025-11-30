using EcommerceApp.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;

public interface ICommandRepository<out TContext, TEntity, in TPrimaryKey> : IRepositoryBase<TContext> where TContext : DbContext where TEntity : class, IEntityBase<TPrimaryKey>
{
    DbSet<TEntity> DbSet { get; }

    void AttachIfNot(TEntity entity);

    TEntity Insert(TEntity entity);

    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

    List<TEntity> InsertMany(IEnumerable<TEntity> entities);

    Task<List<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));


    TEntity Update(TEntity entity);


    void Delete(TEntity entity);

    void Delete(TPrimaryKey id);

    void DeleteMany(IEnumerable<TEntity> entities);
}