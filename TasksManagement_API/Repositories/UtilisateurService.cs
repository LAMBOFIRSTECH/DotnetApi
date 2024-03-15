using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Repositories
{
	public class UtilisateurService : IReadUsersMethods, IWriteUsersMethods
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		public UtilisateurService(DailyTasksMigrationsContext dataBaseMemoryContext)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
		}
		public async Task<List<Utilisateur>> GetUsers()
		{
			var listUtilisateur = await dataBaseMemoryContext.Utilisateurs.ToListAsync();
			await dataBaseMemoryContext.SaveChangesAsync();

			return listUtilisateur;
		}
		public async Task<Utilisateur> GetUserById(int id)
		{
			var utilisateur = await dataBaseMemoryContext.Utilisateurs.FirstOrDefaultAsync(u => u.ID == id);
			return utilisateur;
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
		public async Task<Utilisateur> UpdateUser(Utilisateur utilisateur)
		{
			var user = await dataBaseMemoryContext.Utilisateurs.FindAsync(utilisateur.ID);
			dataBaseMemoryContext.Utilisateurs.Remove(user);
			Utilisateur utilisateur1 = new()
			{ ID = utilisateur.ID, Nom = utilisateur.Nom, Role = utilisateur.Role, Email = utilisateur.Email };
			var password = utilisateur1.Pass;
			var email = utilisateur1.Email;
			if (!string.IsNullOrEmpty(password))
			{
				utilisateur1.SetHashPassword(password);
			}
			if (!utilisateur1.CheckEmailAdress(email))
			{
				throw new ArgumentException("Adresse e-mail invalide");
			}
			if (utilisateur1.CheckHashPassword(password) && utilisateur1.CheckEmailAdress(email))
			{
				dataBaseMemoryContext.Utilisateurs.Add(utilisateur1);
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			return utilisateur1;
		}

		public async Task<Utilisateur> PartialUpdateUser(int id, string nom, string mdp, string role, string email)
		{

			var user = await dataBaseMemoryContext.Utilisateurs.FindAsync(id);
			if (user == null)
			{
				throw new InvalidOperationException("Utilisateur non trouvé");
			}
			if (!string.IsNullOrEmpty(nom))
			{
				user.Nom = nom;
			}
			if (!string.IsNullOrEmpty(mdp))
			{
				string hashedPassword = BCrypt.Net.BCrypt.HashPassword(mdp);
				if (BCrypt.Net.BCrypt.Verify(mdp, user.Pass))
				{
					user.Pass = hashedPassword;
				}
			}
			if (!string.IsNullOrEmpty(role))
			{
				if (Enum.TryParse(role, out Utilisateur.Privilege roleEnum))
				{
					user.Role = roleEnum;
				}
				else
				{
					throw new ArgumentException("Rôle non valide");
				}
			}
			if (!string.IsNullOrEmpty(email))
			{
				user.Email = email;
			}
			if (user.CheckHashPassword(mdp) && user.CheckEmailAdress(email))
			{
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			return user;
		}
	}
}