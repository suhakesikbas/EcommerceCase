using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceApp.Shared.Infrastructure.EntityFramework;

public class RepositoryBase<TContext> : IRepositoryBase<TContext> where TContext : DbContext
{
    public TContext Context { get; }

    public RepositoryBase(TContext context)
    {
        Context = context;
    }

    public int Save()
    {
        return Context.SaveChanges();
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }
}