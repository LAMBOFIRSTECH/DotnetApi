using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Rewrite;
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
	opt.SwaggerDoc("1.0", new OpenApiInfo
	{
		Title = "DailyTasks | Api",
		Description = "An ASP.NET Core Web API for managing Tasks App",
		Version = "2.0",
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
						  policy.WithOrigins("https://localhost:7250", "http://localhost:5195/");
					  });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<IReadUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IWriteUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IReadTasksMethods, TacheService>();
builder.Services.AddScoped<IWriteTasksMethods, TacheService>();
builder.Services.AddTransient<IJwtTokenService, JwtBearerAuthentificationService>();
builder.Services.AddLogging();
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
		 policy.RequireRole(nameof(Utilisateur.Privilege.Admin))
			   .RequireAuthenticatedUser()
			   .AddAuthenticationSchemes("JwtAuthorization"));


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
	app.UseSwagger();
	app.UseSwaggerUI(con =>
	 {
		 con.SwaggerEndpoint("/swagger/2.0/swagger.json", "Daily Tasks Management API");

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
			await context.Response.WriteAsync("Version de l'API : 2.0");
		});
 });
app.Run();