using Microsoft.AspNetCore.Http;
using AspNetWebApiRecipe.Models;
using AspNetWebApiRecipe.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AspNetWebApiRecipe.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void RollbackChanges();
    }
}
