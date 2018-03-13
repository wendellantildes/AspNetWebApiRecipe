using System;
namespace AspNetWebApiRecipe.Repositories
{
    public interface IRepository<T> where T : class
    {
        T Get(object id);
        void Add(T item);
        void Remove(T item);
        void Update(T item);
    }
}
