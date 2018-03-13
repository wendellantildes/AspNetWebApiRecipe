using System;
using AspNetWebApiRecipe.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AspNetWebApiRecipe.Repositories
{
    public class EFUnitOfWork : IQueryableUnitOfWork
    {
        public DbContext context
        {
            get;
            private set;
        }

        public EFUnitOfWork(AspNetWebApiRecipeContext context)
        {
            this.context = context;
        }


        public IQueryable Query<T>() where T : class{
            return this.context.Set<T>().AsQueryable();
        }

        public DbSet<T> CreateSet<T>() where T : class
        {
            return context.Set<T>();
        }

        public void Attach<T>(T item) where T : class
        {
            context.Attach<T>(item);
        }

        public void SetModified<T>(T item) where T : class
        {
            context.Entry<T>(item).State = EntityState.Modified;
        }

        public void ApplyCurrentValues<T>(T original, T current) where T : class
        {
            context.Entry<T>(original).CurrentValues.SetValues(current);
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
            GC.SuppressFinalize(this);
        }

        public void RollbackChanges()
        {
            context.ChangeTracker.Entries().ToList().ForEach((entry => entry.State = EntityState.Unchanged));
        }
    }
}
