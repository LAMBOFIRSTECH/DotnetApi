using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
namespace TasksManagement_API.ServicesRepositories
{
	public class UtilisateurService : IReadUsersMethods, IWriteUsersMethods
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		private readonly IDataProtectionProvider provider;
		private readonly IJwtTokenService jwtTokenService;
		private readonly ILogger<UtilisateurService> logger;
		private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
		private const string Purpose = "my protection purpose"; //On donne une intention pour l'encryptage explire dans 90jours
		public UtilisateurService(DailyTasksMigrationsContext dataBaseMemoryContext, ILogger<UtilisateurService> logger, IJwtTokenService jwtTokenService, Microsoft.Extensions.Configuration.IConfiguration configuration, IDataProtectionProvider provider)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
			this.jwtTokenService = jwtTokenService;
			this.configuration = configuration;
			this.provider = provider;
			this.logger = logger;
		}

		public async Task<TokenResult> GetToken(string email)
		{
			var utilisateur = dataBaseMemoryContext.Utilisateurs
				.SingleOrDefault(u => u.Email.ToUpper().Equals(email.ToUpper()) && u.Role.Equals(Utilisateur.Privilege.Admin));

			if (utilisateur is null)
			{
				return new TokenResult
				{
					Success = false,
					Message = "Droits insuffisants ou adresse mail inexistante !"
				};
			}
			await Task.Delay(500);
			return new TokenResult
			{
				Success = true,
				Token = jwtTokenService.GenerateJwtToken(utilisateur.Email)
			};

		}

		public bool CheckUserSecret(string secretPass)
		{
			string secretUserPass = configuration["TasksManagement_API:SecretApiKey"];
			if (string.IsNullOrEmpty(secretPass))
			{
				throw new NotImplementedException("La clé secrete est inexistante");

			}
			var Pass = BCrypt.Net.BCrypt.HashPassword($"{secretPass}");

			var BCryptResult = BCrypt.Net.BCrypt.Verify(secretUserPass, Pass);
			if (!BCryptResult.Equals(true)) { return false; }
			return true;
		}
		public async Task<List<Utilisateur>> GetUsers()
		{
			var listUtilisateur = await dataBaseMemoryContext.Utilisateurs.ToListAsync();
			await dataBaseMemoryContext.SaveChangesAsync();

			return listUtilisateur;
		}

		public async Task<Utilisateur> GetUserById(int id)
		{
			var utilisateur = dataBaseMemoryContext.Utilisateurs.FirstOrDefault(u => u.ID == id);
			await Task.Delay(200);
			return utilisateur!;
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
				await dataBaseMemoryContext.Utilisateurs.AddAsync(utilisateur);
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			return utilisateur;
		}

		public async Task<Utilisateur> SetUserPassword(string nom, string mdp)
		{
			var adminUser = await dataBaseMemoryContext.Utilisateurs!
			.Where(u => u.Role!.Equals(Utilisateur.Privilege.Admin))
			.Select(u => u.Nom).ToListAsync();

			var user = await dataBaseMemoryContext.Utilisateurs!.Where(u => u.Nom!.Equals(nom)).SingleOrDefaultAsync();
			if (user == null)
			{
				throw new InvalidOperationException("Utilisateur non trouvé");
			}
			if (!user.CheckHashPassword(mdp))
			{
				user.Pass = user.SetHashPassword(mdp);

				logger.LogInformation($"################################### Le mot de passe de l'utilisateur [{nom}] a été changé par l'admin {adminUser.OrderBy(u => Guid.NewGuid()).FirstOrDefault()}");
				logger.LogInformation($"################################### Voici le nombre des utilisateurs admin {adminUser.Count()}");
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			return user;
		}

		public async Task DeleteUserById(int id)
		{
			var result = await dataBaseMemoryContext.Utilisateurs.FirstOrDefaultAsync(u => u.ID == id);
			if (result != null)
			{
				dataBaseMemoryContext.Utilisateurs.Remove(result);
				await dataBaseMemoryContext.SaveChangesAsync();
			}
		}
	}
}