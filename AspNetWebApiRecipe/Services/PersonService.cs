using System;
using AspNetWebApiRecipe.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using AspNetWebApiRecipe.DTOS;
using System.Collections.Generic;
using System.Collections;
using AspNetWebApiRecipe.Repositories;

namespace AspNetWebApiRecipe.Services
{

    public interface IPersonService
    {
        PersonDTO Create(PersonDTO person);
        PersonDTO Get(int id);
        IEnumerable<PersonDTO> GetAll();
        void Remove(int id);
        PersonDTO Update(int id, PersonDTO person);

    }
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository personRepository;

        public PersonService(IPersonRepository personRepository) {
            this.personRepository = personRepository;
        } 

        public PersonDTO Update(int id, PersonDTO personDTO)
        {
            var person = this.personRepository.Get(id);
            person.Name = personDTO.Name;
            this.personRepository.Update(person);
            this.personRepository.UnitOfWork.Commit();
            return AutoMapper.Mapper.Map<PersonDTO>(this.personRepository.Query().Single(x => x.Id == id));
        }

        public PersonDTO Create(PersonDTO dto)
        {
            var person = AutoMapper.Mapper.Map<Person>(dto);
            this.personRepository.Add(person);
            this.personRepository.UnitOfWork.Commit();
            return AutoMapper.Mapper.Map<PersonDTO>(person);
      
        }

        public PersonDTO Get(int id)
        {
            var person = this.personRepository.Query().SingleOrDefault(x => x.Id == id);
            return person == null ? null : AutoMapper.Mapper.Map<PersonDTO>(person);
        }

        public IEnumerable<PersonDTO> GetAll()
        {
            return AutoMapper.Mapper.Map<IEnumerable<PersonDTO>>(this.personRepository.GetAll());
        }

        public void Remove(int id)
        {
            this.personRepository.Remove(x => x.Id == id);
            this.personRepository.UnitOfWork.Commit();
        }
    }
}
