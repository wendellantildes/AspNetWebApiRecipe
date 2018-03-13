using System.Collections.Generic;
using AspNetWebApiRecipe.DTOS;
using AspNetWebApiRecipe.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetWebApiRecipe.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        private readonly IPersonService personService;

        public PersonController(IPersonService personService)
        {
            this.personService = personService;
        }

        [HttpGet]
        public IEnumerable<PersonDTO> Get()
        {
            return this.personService.GetAll();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var person = this.personService.Get(id);
            if(person == null){
                return this.NotFound();
            }else{
                return this.Ok(person);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]PersonDTO value)
        {
            var person = this.personService.Create(value);
            return this.Created("/person/{id}", person);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]PersonDTO value)
        {
            var person = this.personService.Update(id, value);
            return this.Ok(person);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            this.personService.Remove(id);
            return this.NoContent();

        }
    }
}
