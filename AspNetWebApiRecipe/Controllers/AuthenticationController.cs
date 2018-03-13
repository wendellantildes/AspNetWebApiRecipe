using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AspNetWebApiRecipe.DTOS;
using AspNetWebApiRecipe.Services;
using AspNetWebApiRecipe;

namespace AspNetWebApiRecipe.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService){
            this.authenticationService = authenticationService;
        }

        /// <summary>
        /// Do an user's authentication.
        /// </summary>
        /// <returns>The post.</returns>
        /// <param name="authentication"></param>
        [HttpPost]
        public ActionResult Post([FromBody]AutehnticationRequestDTO authentication)
        {
            return Ok(this.authenticationService.Auth(authentication));
        }
    }
}
