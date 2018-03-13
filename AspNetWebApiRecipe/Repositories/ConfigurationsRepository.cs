using System;
using Microsoft.AspNetCore.Hosting;

namespace AspNetWebApiRecipe.Repositories
{

    public interface IConfigurationsRepository{
        bool IsDevEnvironment();
    }

    public class ConfigurationsRepository : IConfigurationsRepository
    {
        private IHostingEnvironment env;

        public ConfigurationsRepository(IHostingEnvironment env){
            this.env = env;
        }

        public bool IsDevEnvironment(){
            return this.env.IsDevelopment();
        }
    }
}
