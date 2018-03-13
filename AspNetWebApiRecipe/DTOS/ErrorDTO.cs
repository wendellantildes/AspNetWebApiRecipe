using System;
namespace AspNetWebApiRecipe.DTOS
{
    public class ErrorDTO
    {
        public string Code { get; private set; }
        public string Message { get; private set; }

        public ErrorDTO(string codigo, string mensagem){
            this.Code = codigo;
            this.Message = mensagem;
        }

    }
}
