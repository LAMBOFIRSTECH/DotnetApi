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
		public async Task<List<Tache>> GetTaches(Func<IQueryable<Tache>, IQueryable<Tache>> filter = null)
		{
			IQueryable<Tache> query = dataBaseSqlServerContext.Taches;
			if (filter != null)
			{
				query = filter(query);
			}
			return await query.ToListAsync();
		}
		public async Task<Tache> GetTaskByTitle(string titre)
		{
			return (await GetTaches(query => query.Where(t => t.Titre.Equals(titre)))).FirstOrDefault();
		}
		public async Task<Tache> CreateTask(Tache tache)
		{
			if (tache.StartDateH.Date >= tache.EndDateH.Date)
			{
				var message = "Exemple : Date de debut ->  01/01/2024  (doit etre '>' Supérieur) Date de fin -> 02/02/2024";
				throw new ArgumentException("Date error", message);
			}
			await dataBaseSqlServerContext.Taches.AddAsync(tache);
			await dataBaseSqlServerContext.SaveChangesAsync();
			return tache;
		}

		public async Task DeleteTaskByTitle(string titre)
		{
			var result = await GetTaches(query => query.Where(t => t.Titre.Equals(titre)));
			if (result.FirstOrDefault() != null)
			{
				dataBaseSqlServerContext.Taches.Remove(result.FirstOrDefault());
				await dataBaseSqlServerContext.SaveChangesAsync();
			}
		}

		public async Task<Tache> UpdateTask(Tache tache)
		{
			var tache1 = await GetTaches(query => query.Where(t => t.Titre.Equals(tache.Titre)));
			dataBaseSqlServerContext.Taches.Remove(tache1.First());
			Tache newtache = new()
			{
				// Matricule = tache.Matricule,
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH = tache.EndDateH
			};
			await dataBaseSqlServerContext.Taches.AddAsync(newtache);
			await dataBaseSqlServerContext.SaveChangesAsync();
			return newtache;
		}


	}
	/**
	- Changer la propriété qui permet de rechercher de façon unique une tache.
	- La fonction update ne prends pas en considération les date de debut et fin.
	- Mettre la logique de vérification de la date dans le controller et pas dans le service.
	- Revoir la fonction getByTitle pour rajouter une regex qui prend en considération le fait que deux titres peuvent commencer de la meme façon sans pour autant etre identique.
	**/ 
}