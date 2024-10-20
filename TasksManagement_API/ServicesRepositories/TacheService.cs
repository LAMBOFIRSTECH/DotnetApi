using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace TasksManagement_API.ServicesRepositories
{
	public class TacheService : IReadTasksMethods, IWriteTasksMethods
	{
		private readonly DailyTasksMigrationsContext dataBaseSqlServerContext;
		public TacheService(DailyTasksMigrationsContext dataBaseSqlServerContext)
		{
			this.dataBaseSqlServerContext = dataBaseSqlServerContext;
		}
		/// <summary>
		/// Renvoie la liste des taches.
		/// </summary>
		/// <returns></returns>

		public async Task<ICollection<Tache>> GetTaches(Func<IQueryable<Tache>, IQueryable<Tache>> filter = null)
		{
			IQueryable<Tache> query = dataBaseSqlServerContext.Taches;
			if (filter != null)
			{
				query = filter(query);
			}
			return await query.ToListAsync();
		}

		public async Task<Tache> CreateTask(Tache tache)
		{
			//var utilisateur = await dataBaseSqlServerContext.Utilisateurs.FindAsync(tache.UserId);
			var utilisateur = await dataBaseSqlServerContext.Utilisateurs
                        .Include(u => u.LesTaches)  // Inclure les tâches associées à l'utilisateur
                        .FirstOrDefaultAsync(u => u.ID == tache.UserId);

			if (utilisateur == null)
			{
				throw new ArgumentException("L'utilisateur associé à la tâche n'existe pas.");
			}
			utilisateur.LesTaches.Add(tache); // Initialiser la liste de tâches si elle est null
			tache.utilisateur = utilisateur;  // Associer la tâche à l'utilisateur
			await dataBaseSqlServerContext.Taches.AddAsync(tache);
			await dataBaseSqlServerContext.SaveChangesAsync();
			return tache;
		}

		public async Task DeleteTaskByTitle(string titre)
		{
			var Taches = await GetTaches(query => query.Where(t => t.Titre.Equals(titre)));
			if (Taches.FirstOrDefault() != null)
			{
				dataBaseSqlServerContext.Taches.Remove(Taches.FirstOrDefault());
				await dataBaseSqlServerContext.SaveChangesAsync();
			}
		}

		public async Task<Tache> UpdateTask(int matricule, Tache tache)
		{
			var Taches = await GetTaches(query => query.Where(t => t.Matricule.Equals(matricule)));
			var newTache = Taches.First();
			// newTache.Matricule = tache.Matricule;
			newTache.Titre = tache.Titre;
			newTache.Summary = tache.Summary;
			newTache.StartDateH = tache.StartDateH;
			newTache.EndDateH = tache.EndDateH;
			await dataBaseSqlServerContext.SaveChangesAsync();
			return newTache;
		}
	}
	/*
	- Changer la propriété qui permet de rechercher de façon unique une tache. Fait
	- La fonction update ne prends pas en considération les date de debut et fin. Fait
	- Mettre la logique de vérification de la date dans le controller et pas dans le service. Fait
	- Revoir la fonction getByTitle pour rajouter une regex qui prend en considération le fait que deux titres peuvent commencer de la meme façon sans pour autant etre identique.
	*/
}