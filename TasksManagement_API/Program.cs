using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.ServicesRepositories;
using TasksManagement_API.Authentifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using DotNetEnv;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("1.1", new OpenApiInfo
	{
		Title = "DailyTasks | Api",
		Description = "An ASP.NET Core Web API for managing Tasks App",
		Version = "1.1",
		Contact = new OpenApiContact
		{
			Name = "Artur Lambo",
			Email = "lamboartur94@gmail.com"
		}
	});
	// opt.OperationFilter<RemoveParameterFilter>();
	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
					  policy =>
					  {
						  policy.AllowAnyOrigin()
						   .AllowAnyMethod()
						   .AllowAnyHeader();
					  });
});


Env.Load();

var environment = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
	  .AddJsonFile(environment, optional: false, reloadOnChange: true);

var connectionStrings = builder.Configuration.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();
if (connectionStrings == null || !connectionStrings.ContainsKey("DefaultConnection"))
{
	throw new Exception("La chaine de connexion à la base de données est nulle");
}

builder.Services.AddDbContext<DailyTasksMigrationsContext>(opt => opt.UseSqlServer(connectionStrings["DefaultConnection"], sqlOptions => sqlOptions.EnableRetryOnFailure(
maxRetryCount: 10,
maxRetryDelay: TimeSpan.FromSeconds(40),
errorNumbersToAdd: null)));

builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();

var certificateFile = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path");
var certificateKey = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Key");

builder.Services.Configure<KestrelServerOptions>(options =>
{
	if (string.IsNullOrEmpty(certificateFile) || string.IsNullOrEmpty(certificateKey))
	{
		throw new InvalidOperationException("Le chemin du certificat ou sa clé privée n'est pas configuré");
	}
	var certificate = X509Certificate2.CreateFromPemFile(certificateFile, certificateKey);
	options.ListenAnyIP(5195);
	options.ListenAnyIP(7251, listenOptions =>
	{
		listenOptions.UseHttps(certificate);
	});
	options.Limits.MaxConcurrentConnections = 5;
	options.ConfigureHttpsDefaults(opt =>
	{
		opt.ClientCertificateMode = ClientCertificateMode.RequireCertificate;

	});
});


builder.Services.AddScoped<IReadUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IWriteUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IReadTasksMethods, TacheService>();
builder.Services.AddScoped<IWriteTasksMethods, TacheService>();
builder.Services.AddTransient<IJwtTokenService, JwtBearerAuthentificationService>();
builder.Services.AddAuthorization();


builder.Services.AddAuthentication("BasicAuthentication")
	.AddScheme<AuthenticationSchemeOptions, AuthentificationBasic>("BasicAuthentication", options => { });
builder.Services.AddAuthentication("JwtAuthorization")
	.AddScheme<JwtBearerOptions, JwtBearerAuthorizationServer>("JwtAuthorization", options =>
	{
		var JwtSettings = builder.Configuration.GetSection("JwtSettings");
		var secretKeyLength = int.Parse(JwtSettings["JwtSecretKey"]);
		var randomSecretKey = new RandomUserSecret();
		var signingKey = randomSecretKey.GenerateRandomKey(secretKeyLength);

		options.SaveToken = true;
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = JwtSettings["Issuer"],
			ValidAudience = JwtSettings["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
		};
	});


builder.Services.AddAuthorization(options =>
 {
	 // Politique d'autorisation pour les administrateurs
	 options.AddPolicy("AdminPolicy", policy =>
		 policy.RequireRole(nameof(Utilisateur.Privilege.Administrateur))
			   .RequireAuthenticatedUser()
			   .AddAuthenticationSchemes("JwtAuthorization"));


	 // Politique d'autorisation pour les utilisateurs non-administrateurs
	 options.AddPolicy("UserPolicy", policy =>
		policy.RequireRole(nameof(Utilisateur.Privilege.Utilisateur))
			   .RequireAuthenticatedUser()  // L'utilisateur doit être authentifié
			   .AddAuthenticationSchemes("BasicAuthentication"));

 });

var app = builder.Build();

if (app.Environment.IsProduction())
{
	// Gérer les erreurs dans un environnement de production
	app.UseExceptionHandler("/Error");
	app.UseHsts();
	app.UseSwagger();
	app.UseSwaggerUI(con =>
	 {
		 con.SwaggerEndpoint("/swagger/1.1/swagger.json", "Daily Tasks Management API");

		 con.RoutePrefix = string.Empty;

	 });
}

app.UseCors(MyAllowSpecificOrigins);
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
			await context.Response.WriteAsync("Version de l'API : 1.1");
		});
 });
app.Run();