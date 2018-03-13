using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AspNetWebApiRecipe.DTOS
{
    public class AutehnticationRequestDTO
    {
        
        [Required]
        public string User { get; set; }
       
        [Required]
        public string Password { get; set; }
    }

    public class AuthenticationResponseDTO 
    {
        public string Token { get; set; }
    }

}
