using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.ServicesRepositories;
using TasksManagement_API.Authentifications;
using TasksManagement_API.SwaggerFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tasks_WEB_API.SwaggerFilters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;

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
						  policy.WithOrigins("https://lambo.net:7082", "http://lambo.net:5163/");
					  });
});

// Charge les configurations à partir de l'environnement spécifier à ASPNETCORE_ENVIRONMENT 

builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: true);
builder.Services.AddDbContext<DailyTasksMigrationsContext>(opt =>
{
	var item = builder.Configuration.GetSection("TasksManagement_API");
	var conStrings = item["DefaultConnection"];

	opt.UseInMemoryDatabase(conStrings);
});

builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();

// Kestrel -> serveur web par defaut dans aspnet :Cest le gestionnaires des connexions entrantes y compris les connexions en https : On va spécifier le certificat à utiliser pour les connection en HTTPS.
builder.Services.Configure<KestrelServerOptions>(options =>
{
	var kestrelSection = builder.Configuration.GetSection("Kestrel:EndPoints:Https");
	var certificateFile = kestrelSection["Certificate:File"];
	var certificatePassword = kestrelSection["Certificate:Password"];
	var host = Dns.GetHostEntry("lambo.net");
	options.Listen(host.AddressList[0], 7083, listenOptions =>
	{
		listenOptions.UseHttps(certificateFile, certificatePassword);
		
	});
	options.Limits.MaxConcurrentConnections = 5;
	options.ConfigureHttpsDefaults(opt =>
	{
		opt.ClientCertificateMode = ClientCertificateMode.RequireCertificate; // le client doit fournir un certificaat valid pour que l'authentification réussit
	});
});

builder.Services.AddScoped<RemoveParametersInUrl>();
builder.Services.AddScoped<IRemoveParametersIn, RemoveParametersInUrl>();
builder.Services.AddScoped<IReadUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IWriteUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IReadTasksMethods, TacheService>();
builder.Services.AddScoped<IWriteTasksMethods, TacheService>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthorization();

// On va ajouter l'authentification via un certificat SSL/TLS avec le nom "CertificateAuthentication" pour l'access token
builder.Services.AddAuthentication("CertificateAuthentication")
   .AddCertificate()
   .AddCertificateCache(options =>
		{
			options.CacheSize = 1024;
			options.CacheEntryExpiration = TimeSpan.FromMinutes(2); // Activer la mise en cache pour des besoins de performances
		})

	.AddScheme<CertificateAuthenticationOptions, AuthenticationCertification>("CertificateAuthentication", options =>
	{
		options.AllowedCertificateTypes = CertificateTypes.All;
		options.ChainTrustValidationMode = X509ChainTrustMode.System;
		options.ValidateValidityPeriod = true;

		options.Events = new CertificateAuthenticationEvents()
		{
			OnAuthenticationFailed = context =>
			   {
				   var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationCertification>>();
				   logger.LogError($"Authentication failed: {context.Exception}");


				   return Task.CompletedTask;
			   }
		};
	});


// On va ajouter l'authentification basic avec le nom "BasicAuthentication" sans options
builder.Services.AddAuthentication("BasicAuthentication")
	.AddScheme<AuthenticationSchemeOptions, AuthentificationBasic>("BasicAuthentication", options => { });

// On va ajouter l'authentification JWT Bearer avec le nom "JwtAuthentification"
builder.Services.AddAuthentication("JwtAuthentification")
	// On va ajouter le schéma d'authentification personnalisé JwtBearer avec les options par défaut
	.AddScheme<JwtBearerOptions, JwtBearerAuthentification>("JwtAuthentification", options =>
	{
		var JwtSettings = builder.Configuration.GetSection("JwtSettings");
		var secretKeyLength = int.Parse(JwtSettings["JwtSecretKey"]);
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

	 //  options.AddPolicy("CertPolicy", policy =>
	 //  policy.AuthenticationSchemes.Add("CertificateAuthentication"));

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
	app.UseSwagger();
	app.UseSwaggerUI(con =>
	 {
		 con.SwaggerEndpoint("/swagger/1.0/swagger.json", "Daily Tasks Management API");

		 con.RoutePrefix = string.Empty;

	 });
}

//app.UseCors(MyAllowSpecificOrigins);
var rewriteOptions = new RewriteOptions()
	.AddRewrite(@"^index\.html$", "https://lambo.net/index.html", true)
	.AddRedirectToHttpsPermanent();
app.UseRewriter(rewriteOptions);


app.UseHttpsRedirection();
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