using System;
using AspNetWebApiRecipe.Models;

namespace AspNetWebApiRecipe.Repositories
{
    public class PersonRepository : EntityRepository<Person>, IPersonRepository
    {
        public PersonRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
