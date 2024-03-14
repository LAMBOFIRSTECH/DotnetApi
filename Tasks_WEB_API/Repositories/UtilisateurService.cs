using Microsoft.EntityFrameworkCore;
using Tasks_WEB_API.Interfaces;
using Tasks_WEB_API.Models;
namespace Tasks_WEB_API.Repositories
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
			if (!string.IsNullOrEmpty(password))
			{
				utilisateur.SetHashPassword(password);
			}
			var check = utilisateur.CheckHashPassword(password);
			if (check)
			{
				await dataBaseMemoryContext.Utilisateurs.AddAsync(utilisateur);
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			else
			{
				throw new Exception("fake password");
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
			{ ID = utilisateur.ID, Nom = utilisateur.Nom, Pass = utilisateur.Pass, Role = utilisateur.Role ,Email=utilisateur.Email};
			var password = utilisateur1.Pass;
			if (!string.IsNullOrEmpty(password))
			{
				utilisateur1.SetHashPassword(password);
			}
			var check = utilisateur1.CheckHashPassword(password);
			if (check)
			{
				dataBaseMemoryContext.Utilisateurs.Add(utilisateur1);
				await dataBaseMemoryContext.SaveChangesAsync();
			}
			return user;
		}
	}
}