using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.AspNetCore.DataProtection;
namespace TasksManagement_API.ServicesRepositories
{
	public class UtilisateurService : IReadUsersMethods, IWriteUsersMethods
	{
		private readonly DailyTasksMigrationsContext dataBaseSqlServerContext;
		private readonly IDataProtectionProvider provider;
		private readonly IJwtTokenService jwtTokenService;

		private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
		private const string Purpose = "my protection purpose"; //On donne une intention pour l'encryptage explire dans 90jours
		public UtilisateurService(DailyTasksMigrationsContext dataBaseSqlServerContext, IJwtTokenService jwtTokenService, Microsoft.Extensions.Configuration.IConfiguration configuration, IDataProtectionProvider provider)
		{
			this.dataBaseSqlServerContext = dataBaseSqlServerContext;
			this.jwtTokenService = jwtTokenService;
			this.configuration = configuration;
			this.provider = provider;
		}

		public async Task<TokenResult> GetToken(string email)
		{
			var utilisateurs = await GetUsers(query => query.Where(u => u.Email.ToUpper().Equals(email.ToUpper()) && u.Role.Equals(Utilisateur.Privilege.Administrateur)));
			if (utilisateurs is null || utilisateurs.Count == 0)
			{
				return new TokenResult
				{
					Success = false,
					Message = "Droits insuffisants ou adresse mail inexistante !"
				};
			}
			await Task.Delay(500);
			var utilisateur = utilisateurs.First();
			return new TokenResult
			{
				Success = true,
				Token = jwtTokenService.GenerateJwtToken(utilisateur.Email)
			};
		}

		public bool CheckUserSecret(string secretPass)
		{
			//Env.Load("ServicesRepositories/.env");
			string secretUserPass = configuration["JwtSettings:JwtSecretKey"]; // Prod Environment.GetEnvironmentVariable("PasswordSecret")!; //

			if (string.IsNullOrEmpty(secretUserPass))
			{
				throw new NotImplementedException("La clé secrete est inexistante");

			}
			var Pass = BCrypt.Net.BCrypt.HashPassword($"{secretPass}");

			var BCryptResult = BCrypt.Net.BCrypt.Verify(secretUserPass, Pass);
			if (!BCryptResult.Equals(true)) { return false; }
			return true;
		}

		public async Task<ICollection<Utilisateur>> GetUsers(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null)
		{
			IQueryable<Utilisateur> query = dataBaseSqlServerContext.Utilisateurs;
			if (filter != null)
			{
				query = filter(query);
			}
		
			return await query.ToListAsync();
		}
		public async Task<Utilisateur?> GetSingleUserByNameRole(string nom, Utilisateur.Privilege role)
		{
			return ( await GetUsers(query => query.Where(user => user.Nom == nom && user.Role == role))).FirstOrDefault();
		}

		public string EncryptUserSecret(string plainText)
		{
			var protector = provider.CreateProtector(Purpose);
			return protector.Protect(plainText);
		}

		public string DecryptUserSecret(string cipherText)
		{
			var protector = provider.CreateProtector(Purpose);
			return protector.Unprotect(cipherText);
		}
		public async Task<Utilisateur> CreateUser(Utilisateur utilisateur)
		{
			var password = utilisateur.Pass;
			var email = utilisateur.Email;
			if (!string.IsNullOrEmpty(password))
			{
				utilisateur.SetHashPassword(password);
			}

			if (!utilisateur.CheckEmailAdress(email))
			{
				throw new ArgumentException("Adresse e-mail invalide");
			}
			if (utilisateur.CheckHashPassword(password) && utilisateur.CheckEmailAdress(email))
			{
				await dataBaseSqlServerContext.Utilisateurs.AddAsync(utilisateur);
				await dataBaseSqlServerContext.SaveChangesAsync();
			}
			return utilisateur;
		}

		public async Task<Utilisateur> SetUserPassword(string nom, string mdp)
		{
			var utilisateur = (await GetUsers(query => query.Where(u => u.Nom!.Equals(nom)))).FirstOrDefault();
			if (utilisateur == null)
			{
				throw new InvalidOperationException("Utilisateur non trouvé");
			}
			if (!utilisateur.CheckHashPassword(mdp))
			{
				utilisateur.Pass = utilisateur.SetHashPassword(mdp);
				await dataBaseSqlServerContext.SaveChangesAsync();
			}
			return utilisateur;
		}
		public async Task DeleteUserByDetails(string nom, Utilisateur.Privilege role)
		{
			var result = await GetSingleUserByNameRole(nom, role);
			if (result != null)
			{
				dataBaseSqlServerContext.Utilisateurs.Remove(result);
				await dataBaseSqlServerContext.SaveChangesAsync();
			}
		}
	}
}