using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.ServicesRepositories;
using TasksManagement_API.Authentifications;
using TasksManagement_API.SwaggerFilters;
using TasksManagement_API.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tasks_WEB_API.SwaggerFilters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(opt =>
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
				}
			});
			opt.OperationFilter<RemoveParameterFilter>();
			var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

		});

		builder.Services.AddCors(options =>
		{
			options.AddPolicy(name: MyAllowSpecificOrigins,
							  policy =>
							  {
								  policy.WithOrigins("https://localhost:7082", "http://lambo.lft:5163/");
							  });
		});
		
		// Load configuration from appsettings.json and appsettings.{Environment}.json
	
		//Charge les configurations à partir de l'environnement spécifier à ASPNETCORE_ENVIRONMENT 
		builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: true);
		builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true);
		builder.Services.AddDbContext<DailyTasksMigrationsContext>(opt =>
		{
			string conStrings = builder.Configuration.GetConnectionString("DefaultConnection");

			opt.UseInMemoryDatabase(conStrings);
		});

		builder.Services.AddControllersWithViews();
		builder.Services.AddRouting();
		builder.Services.AddScoped<RemoveParametersInUrl>();
		builder.Services.AddHttpContextAccessor();

		builder.Services.AddHealthChecks();

		builder.Services.AddScoped<IReadUsersMethods, UtilisateurService>();
		builder.Services.AddScoped<IWriteUsersMethods, UtilisateurService>();
		builder.Services.AddScoped<IReadTasksMethods, TacheService>();
		builder.Services.AddScoped<IWriteTasksMethods, TacheService>();
		builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
		builder.Services.AddAuthorization();
		builder.Services.AddAuthentication("BasicAuthentication")
			.AddScheme<AuthenticationSchemeOptions, AuthentificationBasic>("BasicAuthentication", options => { });


		// Ajouter l'authentification JWT avec le nom "JwtAuthentification"
		builder.Services.AddAuthentication("JwtAuthentification")
			// Ajouter le schéma d'authentification personnalisé JwtBearer avec les options par défaut
			.AddScheme<JwtBearerOptions, JwtBearerAuthentification>("JwtAuthentification", options =>
			{
				var JwtSettings = builder.Configuration.GetSection("JwtSettings");
				var secretKeyLength = int.Parse(JwtSettings["SecretKey"]);
				var randomSecretKey = new RandomUserSecret();
				var signingKey = randomSecretKey.GenerateRandomKey(secretKeyLength);

				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,             // Valider l'émetteur (issuer) du jeton
					ValidateAudience = true,           // Valider l'audience du jeton
					ValidateLifetime = true,           // Valider la durée de vie du jeton
					ValidateIssuerSigningKey = true,   // Valider la signature du jeton

					ValidIssuer = JwtSettings["Issuer"],       // Émetteur (issuer) valide
					ValidAudience = JwtSettings["Audience"],   // Audience valide
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)) // Clé de signature
				};
			});


		builder.Services.AddAuthorization(options =>
		 {
			 // Politique d'autorisation pour les administrateurs
			 options.AddPolicy("AdminPolicy", policy =>
				 policy.RequireRole(nameof(Utilisateur.Privilege.Admin))
					   .RequireAuthenticatedUser()
					   .AddAuthenticationSchemes("JwtAuthentification"));


			 // Politique d'autorisation pour les utilisateurs non-administrateurs
			 options.AddPolicy("UserPolicy", policy =>
				 policy.RequireRole(nameof(Utilisateur.Privilege.UserX))
					   .RequireAuthenticatedUser()  // L'utilisateur doit être authentifié
					   .AddAuthenticationSchemes("BasicAuthentication"));

		 });

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(con =>
			 {
				 con.SwaggerEndpoint("/swagger/1.0/swagger.json", "Daily Tasks Management API");

				 con.RoutePrefix = string.Empty;

			 });
		}
		else if (app.Environment.IsStaging())
		{

		}
		else if (app.Environment.IsProduction())
		{
			// Gérer les erreurs dans un environnement de production
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

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
			 endpoints.MapHealthChecks("/health");
			 endpoints.MapGet("/version", async context =>
				{
					await context.Response.WriteAsync("Version de l'API : 1.0");
				});
		 });
		app.Run();
	}
}