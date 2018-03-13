using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StructureMap;
using Microsoft.AspNetCore.Mvc.Filters;
using AspNetWebApiRecipe.Services.Common;
using AspNetWebApiRecipe.Services;
using AspNetWebApiRecipe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using AspNetWebApiRecipe.Mappers;
using AspNetWebApiRecipe.Repositories;
using AspNetWebApiRecipe.Models;
using AspNetWebApiRecipe.DTOS;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;

namespace AspNetWebApiRecipe
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AutoMapperConfiguration.RegistrarMapeamentos();
        }

        public IConfiguration Configuration { get; }
        public Container container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<AspNetWebApiRecipeContext>(options =>
                                            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new SecurityFilter());
                    options.Filters.Add(new GlobalExceptionFilter());
                })
                .AddJsonOptions(options =>
                {
                    var settings = options.SerializerSettings;
                    JsonConfig.Configure(settings);
                });

			services.AddMvc()
				.AddControllersAsServices();
			services.AddSwaggerGen(c =>
			{
                c.SwaggerDoc("v1", new Info { Title = "AspNetWebApiRecipe", Version = "v1" });
                c.DocumentFilter<LowercaseDocumentFilter>();
                c.OperationFilter<AspNetWebApiRecipeResponseType>();
				c.AddSecurityDefinition("JWT Token", new ApiKeyScheme
				{
					Description = "JWT Token",
					Name = "Authorization",
					In = "header"
				});

			});
			services.AddMvcCore()
                    .AddApiExplorer();
            services.AddCors();
            services.AddScoped<IQueryableUnitOfWork, EFUnitOfWork>();
            services.AddScoped<IConfigurationsRepository, ConfigurationsRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IAuthenticationService, AutehnticationService>();
            services.AddScoped<IPersonService, PersonService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.ShowRequestHeaders();
                c.ShowJsonEditor();
                c.EnabledValidator();
                c.SupportedSubmitMethods(new[] { "get", "post", "put", "patch" });
			});
			
            app.UseCors(builder => builder
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
            app.UseMvc();
        }
    }

    #region JsonConfig
    public static class JsonConfig
    {
        public static void Configure(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new PTBRTimeConvertor());
            settings.ContractResolver = new LowercaseContractResolver();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver(),
                Converters = { new PTBRTimeConvertor() }
            };
        }

        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }

        public class PTBRTimeConvertor : DateTimeConverterBase
        {
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (reader.Value != null)
                    if (!string.IsNullOrEmpty(reader.Value.ToString()))
                        return DateTime.Parse(reader.Value.ToString());

                return reader.Value;
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteValue(((DateTime)value).ToString("dd/MM/yyyy"));
            }
        }
    }
    #endregion

    #region SegurancaFilter
    public sealed class SecurityFilter : IAuthorizationFilter
    {
        private readonly string authenticationRoute = "authentication";
        private readonly string homeRoute = "home";
        private const string authorizationKey = "Authorization";

        public static string GetToken(HttpRequest request)
        {
            var authorizationParamValue = request.GetTypedHeaders().Headers[authorizationKey];
            return authorizationParamValue[0]?.Replace("Bearer ", "");
        }

        public void OnAuthorization(AuthorizationFilterContext actionContext)
        {
            var request = actionContext.HttpContext.Request;
            var currentAction = request.Path.Value.Replace("/api/", "");
            if (currentAction != this.authenticationRoute && currentAction != this.homeRoute)
            {
                if (currentAction != this.authenticationRoute)
                {
                    try
                    {
                        var token = GetToken(request);
                        var jwtService = new JWTService();
                        if (!request.Method.Equals("OPTIONS"))
                        {
                            jwtService.ValidateToken(token);

                        }
                        //TODO: verify if token exists on database
                    }
                    catch (BusinessException)
                    {
                        actionContext.Result = new UnauthorizedResult();
                    }
                    catch (Exception ex)
                    {
                        actionContext.Result = new BadRequestResult();
                    }
                }
            }
        }
    }
    #endregion

    #region GlobalExceptionFilter
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
		    var messageException = "Something unexpected has happened. Please, try again later.";
            if (context.Exception is BusinessException)
            {
                messageException = context.Exception.Message;
                context.Result = new BadRequestObjectResult(new ErrorDTO(nameof(BusinessException),messageException));
            }else{
                context.Result = new BadRequestObjectResult(new ErrorDTO("Generic", messageException));
            }
        }
    }
    #endregion

	public class LowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
			var paths = swaggerDoc.Paths;

			//  generate the new keys
			var newPaths = new Dictionary<string, PathItem>();
			var removeKeys = new List<string>();
			foreach (var path in paths)
			{
				var newKey = path.Key.ToLower();
				if (newKey != path.Key)
				{
					removeKeys.Add(path.Key);
					newPaths.Add(newKey, path.Value);
				}
			}

			//  add the new keys
			foreach (var path in newPaths)
			{
				swaggerDoc.Paths.Add(path.Key, path.Value);
			}

			//  remove the old keys
			foreach (var key in removeKeys)
			{
				swaggerDoc.Paths.Remove(key);
			}
        }
    }

	public class AspNetWebApiRecipeResponseType : IOperationFilter
	{
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Produces.Clear();
            operation.Consumes.Clear();
            operation.Produces.Add("application/json");
            operation.Consumes.Add("application/json");
        }
    }
}
