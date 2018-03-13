using AutoMapper;
using AspNetWebApiRecipe.DTOS;
using AspNetWebApiRecipe.Models;
using AspNetWebApiRecipe.Services;
using AspNetWebApiRecipe.Services.Common;
using AspNetWebApiRecipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetWebApiRecipe.Repositories;

namespace AspNetWebApiRecipe.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResponseDTO Auth(AutehnticationRequestDTO request); 
    }

    public class AutehnticationService : IAuthenticationService
    {
        private IJWTService jwtService;

        public AutehnticationService(IJWTService jwtService){
            this.jwtService = jwtService;
        }

        public AuthenticationResponseDTO Auth(AutehnticationRequestDTO request)
        {
            var user = Mapper.Map<User>(request);
            return new AuthenticationResponseDTO
            {
                Token = jwtService.GenerateToken(request.User)
            };
        }
    }
}
