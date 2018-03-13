using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AspNetWebApiRecipe.Repositories
{
    public class EntityRepository<T> : IDatabaseRepository<T> where T : class
    {
        private IQueryableUnitOfWork unitOfWork;
        public IUnitOfWork UnitOfWork { 
            get{
                return this.unitOfWork;
            } 
        }

        public EntityRepository(IQueryableUnitOfWork unitOfWork){
            this.unitOfWork = unitOfWork;
        }

        ~EntityRepository(){
            Dispose(false);
        }

        public DbSet<T> GetSet()
        {
            return this.unitOfWork.CreateSet<T>();
        }


        public void Add(T item)
        {
            this.GetSet().Add(item); 
        }

        public T Get(object id)
        {
            return this.GetSet().Find(id);
        }

        public List<T> GetAll()
        {
            return this.GetSet().ToList<T>();
        }

        public void Merge(T originalItem, T modifiedItem)
        {
            this.unitOfWork.ApplyCurrentValues(originalItem, modifiedItem);
        }

        public void Remove(T item)
        {
            this.GetSet().Remove(item);
        }

        public void Remove(Func<T, bool> predicate)
        {
            this.GetSet()
                .Where(predicate).ToList()
                .ForEach(del => this.GetSet().Remove(del));
        }

        public void Update(T item)
        {
            this.GetSet().Update(item);
        }

        protected bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.unitOfWork != null)
                    {
                        this.unitOfWork.Dispose();
                    }
                }
                this.disposed = true;
            }
        }

        public IQueryable<T> Query()
        {
            return GetSet().AsQueryable();
        }
    }
}
