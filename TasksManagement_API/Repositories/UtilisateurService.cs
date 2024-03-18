using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Repositories
{
	public class UtilisateurService : IReadUsersMethods, IWriteUsersMethods
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		private readonly IJwtTokenService jwtTokenService;
		public UtilisateurService(DailyTasksMigrationsContext dataBaseMemoryContext,IJwtTokenService jwtTokenService)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
			this.jwtTokenService = jwtTokenService;
		}
		public async Task<List<Utilisateur>> GetUsers()
		{
			var listUtilisateur = await dataBaseMemoryContext.Utilisateurs.ToListAsync();
			await dataBaseMemoryContext.SaveChangesAsync();

			return listUtilisateur;
		}
		public async Task<Utilisateur> GetUserById(int id)
		{
			var utilisateur =  dataBaseMemoryContext.Utilisateurs.FirstOrDefault(u => u.ID == id);
			await Task.Delay(200);
			return utilisateur!;
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
		public async Task DeleteUserById(int id)
		{
			var result = await dataBaseMemoryContext.Utilisateurs.FirstOrDefaultAsync(u => u.ID == id);
			if (result != null)
			{
				dataBaseMemoryContext.Utilisateurs.Remove(result);

				await dataBaseMemoryContext.SaveChangesAsync();
			}
		}

		public async Task<Utilisateur> SetUserPassword(string nom, string mdp)
		{
			var user = await dataBaseMemoryContext.Utilisateurs!.Where(u => u.Nom!.Equals(nom)).SingleOrDefaultAsync();
			if (user == null)
			{
				throw new InvalidOperationException("Utilisateur non trouvé");
			}
			if (user.CheckHashPassword(mdp))
			{
				throw new ArgumentException("Ce mot de passe existe déjà. Veuillez le modifier");
			}
			else
			{
				user.Pass = user.SetHashPassword(mdp);
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			return user;
		}

		public async Task<string> GetToken(string email)
		{
			var utilisateur = dataBaseMemoryContext.Utilisateurs
				.Single(u => u.Email.ToUpper().Equals(email.ToUpper()) && u.Role.Equals(Utilisateur.Privilege.Admin));
			if (utilisateur is null)
			{
				throw new Exception("Droits insuffisants ou adresse mail inexistante !");
			}
			await Task.Delay(500);
			return jwtTokenService.GenerateJwtToken(utilisateur.Email);
		}
	}
}