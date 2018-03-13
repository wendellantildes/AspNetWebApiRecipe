using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetWebApiRecipe.Repositories
{
    public interface IDatabaseRepository<T> : IRepository<T>, IDisposable where T : class
    {
        IUnitOfWork UnitOfWork { get; }
        List<T> GetAll();
        void Remove(Func<T, bool> predicate);
        void Merge(T originalItem, T modifiedItem);
        IQueryable<T> Query();
    }
}
