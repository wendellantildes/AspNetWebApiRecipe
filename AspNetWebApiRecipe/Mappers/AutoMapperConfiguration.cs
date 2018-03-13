using AutoMapper;
using AspNetWebApiRecipe.DTOS;
using AspNetWebApiRecipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetWebApiRecipe.Mappers
{
    public class AutoMapperConfiguration
    {
        public static void RegistrarMapeamentos()
        {
            Mapper.Initialize(x =>
            {
                x.AllowNullCollections = true;
                x.CreateMap<AutehnticationRequestDTO, User>().ForMember(ent => ent.Login, dto => dto.MapFrom(src => src.User)).ReverseMap();

				
                x.CreateMap<Person, PersonDTO>()
			        .ReverseMap();
            });
        }
    }
}
