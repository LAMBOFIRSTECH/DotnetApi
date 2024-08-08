using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace TasksManagement_API.ServicesRepositories
{
	public class TacheService :IReadTasksMethods,IWriteTasksMethods
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
		public async Task<List<Tache>> GetTaches()
		{
			var listTache = await dataBaseSqlServerContext.Taches.ToListAsync();
			await dataBaseSqlServerContext.SaveChangesAsync();
			return listTache;
		}

		/// <summary>
		/// Renvoie une tache spécifique en fonction de son matricule
		/// </summary>
		/// <param name="matricule"></param>
		/// <returns></returns>

		public async Task<Tache> GetTaskById(int? matricule)
		{
			var tache = await dataBaseSqlServerContext.Taches.FirstOrDefaultAsync(t => t.Matricule == matricule);
			await Task.Delay(200);
			return tache!;
		}
		public async Task<Tache> CreateTask(Tache tache)
		{
			await dataBaseSqlServerContext.Taches.AddAsync(tache);
			await dataBaseSqlServerContext.SaveChangesAsync();
			return tache;
		}

		public async Task DeleteTaskById(int matricule)
		{
			var result = await dataBaseSqlServerContext.Taches.FirstOrDefaultAsync(t => t.Matricule == matricule);
			if (result != null)
			{
				dataBaseSqlServerContext.Taches.Remove(result);

				await dataBaseSqlServerContext.SaveChangesAsync();
			}
		}

		public async Task<Tache> UpdateTask(Tache tache)
		{
			var tache1 = await dataBaseSqlServerContext.Taches.FindAsync(tache.Matricule);
			dataBaseSqlServerContext.Taches.Remove(tache1);
			Tache newtache = new()
			{
				Matricule = tache.Matricule,
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH= tache.EndDateH
			};
			await dataBaseSqlServerContext.Taches.AddAsync(newtache);
			await dataBaseSqlServerContext.SaveChangesAsync();
			return newtache;
		}
	}
}