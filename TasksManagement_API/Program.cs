using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Repositories;
using TasksManagement_API.Authentifications;
using TasksManagement_API.SwaggerFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace TasksManagement_API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			ConfigureServices(builder.Services, configuration); // Passer IConfiguration à ConfigureServices
			var app = builder.Build();
			Configure(app, builder.Environment);
			app.Run();
		}

		public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			// Configuration des services
			var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("1.0", new OpenApiInfo
				{
					Title = "DailyTasks | Api",
					Description = "An ASP.NET Core Web API for managing Tasks App",
					Version = "1.0",
					Contact = new OpenApiContact
					{
						Name = "Artur Lambo",
						Email = "lamboartur94@gmail.com"
					},
					License = new OpenApiLicense
					{
						Name = "Example License",
						Url = new Uri("https://example.com/license")
					}
				});
				opt.OperationFilter<RemoveParameterFilter>();
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
			});

			services.AddCors(options =>
			{
				options.AddPolicy(name: MyAllowSpecificOrigins,
								  policy =>
								  {
									  policy.WithOrigins("https://localhost:7082", "http://lambo.lft:5163/");
								  });
			});

			services.AddDbContext<DailyTasksMigrationsContext>(opt =>
			{
				string conStrings = configuration.GetConnectionString("DefaultConnection");



				opt.UseInMemoryDatabase(conStrings);
			});

			services.AddScoped<IReadUsersMethods, UtilisateurService>();
			services.AddScoped<IWriteUsersMethods, UtilisateurService>();
			services.AddScoped<IReadTasksMethods, TacheService>();
			services.AddScoped<IWriteTasksMethods, TacheService>();
			services.AddTransient<IJwtTokenService, JwtTokenService>();
			services.AddAuthorization();
			services.AddAuthentication("BasicAuthentication")
				.AddScheme<AuthenticationSchemeOptions, AuthentificationBasic>("BasicAuthentication", options => { });

			// Ajouter l'authentification JWT avec le nom "JwtAuthentification"
			services.AddAuthentication("JwtAuthentification")
				.AddScheme<JwtBearerOptions, JwtBearerAuthentification>("JwtAuthentification", options =>
				{
					// Configuration JWT
				});

			services.AddAuthorization(options =>
			{
				// Politique d'autorisation pour les administrateurs
				options.AddPolicy("AdminPolicy", policy =>
					policy.RequireRole(nameof(Utilisateur.Privilege.Admin))
						  .RequireAuthenticatedUser()
						  .AddAuthenticationSchemes("JwtAuthentification"));

				// Politique d'autorisation pour les utilisateurs non-administrateurs
				options.AddPolicy("UserPolicy", policy =>
					policy.RequireRole(nameof(Utilisateur.Privilege.UserX))
						  .RequireAuthenticatedUser()
						  .AddAuthenticationSchemes("BasicAuthentication"));
			});
		}

		public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(con =>
				 {
					 con.SwaggerEndpoint("/swagger/1.0/swagger.json", "Daily Tasks Management API");
					 con.RoutePrefix = string.Empty;
				 });
			}
			else if (env.IsProduction())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
			app.UseCors(MyAllowSpecificOrigins);
			var rewriteOptions = new RewriteOptions()
					 .AddRewrite(@"^www\.taskmoniroting/Taskmanagement", "https://localhost:7082/index.html", true);
			//app.UseHttpsRedirection();
			app.UseRewriter(rewriteOptions);
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
