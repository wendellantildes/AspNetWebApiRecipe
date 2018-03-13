using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jose;

namespace AspNetWebApiRecipe.Services.Common
{
    public interface IJWTService
    {
        string GenerateToken(string user);
        void ValidateToken(string token);
    }

    public class JWTService : IJWTService
    {
        private readonly string key = "recipe";

        public string GenerateToken(string user)
        {
            var minutes = 60;
            var payload = new Dictionary<string, object>
                {
                    { "sub", user },
                    { "iat", GetTimestamp(DateTime.Now) },
                    { "exp", GetTimestamp(DateTime.Now.AddMinutes(minutes)) }
                };
            var keyAsBytes = Encoding.UTF8.GetBytes(key);

            return Jose.JWT.Encode(payload, keyAsBytes, JwsAlgorithm.HS256);
        }

        public void ValidateToken(string token)
        {
            var keyAsBytes = Encoding.UTF8.GetBytes(key);
            try
            {
                var json = "";
                json = Jose.JWT.Decode(token, keyAsBytes);
                var date = FormatDate(json);
                if(DateTime.Now > date)
                {
                    throw new BusinessException("Token is not valid.");
                }
            }
            catch (IntegrityException)
            {
                throw new BusinessException("Token is not valid.");
            }
        }

        private long GetTimestamp(DateTime value)
        {
            return Convert.ToInt64(value.ToString("yyyyMMddHHmmssffff"));
        }


        private DateTime FormatDate(string date)
        {
            try
            {
                date = date.Split(',')[2].Split(':')[1];
                date = date.Substring(0, date.Length - 4);
                date = $"{date.Substring(0, 4)}-{date.Substring(4, 2)}-{date.Substring(6, 2)}T{date.Substring(8, 2)}:{date.Substring(10, 2)}:{date.Substring(12, 2)}";
                return Convert.ToDateTime(date);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}
