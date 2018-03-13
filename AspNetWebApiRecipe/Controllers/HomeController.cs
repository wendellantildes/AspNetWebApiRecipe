using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetWebApiRecipe.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspNetWebApiRecipe.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IConfigurationsRepository repository;

        public HomeController(IConfigurationsRepository configurationRepository){
            this.repository = configurationRepository;
        }

		[HttpGet]
        public ActionResult Get()
        {
            return Ok(repository.IsDevEnvironment());
        }
    }
}
