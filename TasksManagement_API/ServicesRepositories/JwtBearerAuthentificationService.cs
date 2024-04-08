using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;

namespace TasksManagement_API.Authentifications
{
    public class JwtBearerAuthentificationService : IJwtTokenService
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;

		public JwtBearerAuthentificationService(DailyTasksMigrationsContext dataBaseMemoryContext, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
			this.configuration = configuration;
		}
		public string GetSigningKey()
		{
			var JwtSettings = configuration.GetSection("JwtSettings");
			int secretKeyLength = int.Parse(JwtSettings["JwtSecretKey"]);
			var randomSecretKey = new RandomUserSecret();
			string signingKey = randomSecretKey.GenerateRandomKey(secretKeyLength);
			return signingKey;
		}
		public string GenerateJwtToken(string email)
		{
			var utilisateur = dataBaseMemoryContext.Utilisateurs
			.Single(u => u.Email.ToUpper().Equals(email.ToUpper()) && u.Role.Equals(Utilisateur.Privilege.Admin));
			if (utilisateur is null)
			{
				throw new ArgumentException("Cet utilisateur n'existe pas");
			}
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSigningKey()));
			var tokenHandler = new JwtSecurityTokenHandler();

#pragma warning disable CS8604 // Possible null reference argument.
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] {
					new Claim(ClaimTypes.Name, utilisateur.Nom),
					new Claim(ClaimTypes.Email, utilisateur.Email),
					new Claim(ClaimTypes.Role, utilisateur.Role.ToString())
					}
				),
				Expires = DateTime.UtcNow.AddMinutes(50),
				SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature),
				Audience = configuration.GetSection("JwtSettings")["Audience"],
				Issuer = configuration.GetSection("JwtSettings")["Issuer"],
			};
#pragma warning restore CS8604 // Possible null reference argument.
			var tokenCreation = tokenHandler.CreateToken(tokenDescriptor);
			var token = tokenHandler.WriteToken(tokenCreation);
			return token;
		}
	}
}