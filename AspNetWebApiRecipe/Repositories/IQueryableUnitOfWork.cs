using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AspNetWebApiRecipe.Repositories
{
    public interface IQueryableUnitOfWork : IUnitOfWork
    {
        DbSet<T> CreateSet<T>() where T : class;
        void Attach<T>(T item) where T : class;
        void SetModified<T>(T item) where T : class;
        void ApplyCurrentValues<T>(T original, T current) where T : class;
        IQueryable Query<T>() where T : class;
    }
}
