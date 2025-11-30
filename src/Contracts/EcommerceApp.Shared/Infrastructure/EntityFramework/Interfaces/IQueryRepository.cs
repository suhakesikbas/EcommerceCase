using EcommerceApp.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;

public interface IQueryRepository<out TContext, TEntity, in TPrimaryKey> : IRepositoryBase<TContext> where TContext : DbContext where TEntity : class, IEntityBase<TPrimaryKey>
{
    DbSet<TEntity> DbSet { get; }

    IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>>? predicate = null);

    List<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? GetById(TPrimaryKey id);

    Task<TEntity?> GetByIdAsync(TPrimaryKey id);

    List<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod) where T : class;
}