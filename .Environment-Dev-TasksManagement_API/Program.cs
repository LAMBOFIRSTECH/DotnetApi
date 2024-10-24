using System.Reflection;
using Microsoft.AspNetCore.Authentication;
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
	opt.OperationFilter<RemoveParameterFilter>();
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


builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
optional: true, reloadOnChange: true
);
var item = builder.Configuration.GetSection("ConnectionStrings");
var conStrings = item["DefaultConnection"];
if (conStrings == null)
{
	throw new Exception("La chaine de connection à la base de données est nulle");
}
builder.Services.AddDbContext<DailyTasksMigrationsContext>(opt => opt.UseInMemoryDatabase(conStrings));


builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();

// var kestrelSection=builder.Configuration.GetSection("Kestrel:EndPoints:Https");
// var certificateFile = kestrelSection["Certificate:File"];
// var certificatePassword = kestrelSection["Certificate:Password"];
// builder.Services.Configure<KestrelServerOptions>(options =>
// {
//     if (string.IsNullOrEmpty(certificateFile) || string.IsNullOrEmpty(certificatePassword))
//     {
//         throw new InvalidOperationException("Certificate path or password not configured");
//     }
//     options.ListenAnyIP(7081, listenOptions =>
//     {
//         listenOptions.UseHttps(certificateFile, certificatePassword);
//     });
//     options.Limits.MaxConcurrentConnections = 5;
//     options.ConfigureHttpsDefaults(opt =>
//     {
//         opt.ClientCertificateMode = ClientCertificateMode.RequireCertificate;

//     });
// });


builder.Services.AddScoped<IReadUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IWriteUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IReadTasksMethods, TacheService>();
builder.Services.AddScoped<IWriteTasksMethods, TacheService>();
builder.Services.AddTransient<IJwtTokenService, JwtBearerAuthentificationService>();
builder.Services.AddLogging();
builder.Services.AddAuthorization();

// On va ajouter l'authentification basic avec le nom "BasicAuthentication" sans options
builder.Services.AddAuthentication("BasicAuthentication")
	.AddScheme<AuthenticationSchemeOptions, AuthentificationBasic>("BasicAuthentication", options => { });

// On va ajouter l'authentification JWT Bearer avec le nom "JwtAuthentification"
builder.Services.AddAuthentication("JwtAuthorization")
	// On va ajouter le schéma d'authentification personnalisé JwtBearer avec les options par défaut
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

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(con =>
	 {
		 con.SwaggerEndpoint("/swagger/1.1/swagger.json", "Daily Tasks Management API");

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