using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.EntityFrameworkCore;

namespace TasksManagement_API.ServicesRepositories
{
	public class TacheService :IReadTasksMethods,IWriteTasksMethods
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		public TacheService(DailyTasksMigrationsContext dataBaseMemoryContext)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
		}
		/// <summary>
		/// Renvoie la liste des taches.
		/// </summary>
		/// <returns></returns>
		public async Task<List<Tache>> GetTaches()
		{
			var listTache = await dataBaseMemoryContext.Taches.ToListAsync();
			await dataBaseMemoryContext.SaveChangesAsync();
			return listTache;
		}

		/// <summary>
		/// Renvoie une tache spécifique en fonction de son matricule
		/// </summary>
		/// <param name="matricule"></param>
		/// <returns></returns>

		public async Task<Tache> GetTaskById(int? matricule)
		{
			var tache = await dataBaseMemoryContext.Taches.FirstOrDefaultAsync(t => t.Matricule == matricule);
			await Task.Delay(200);
			return tache!;
		}
		public async Task<Tache> CreateTask(Tache tache)
		{
			await dataBaseMemoryContext.Taches.AddAsync(tache);
			await dataBaseMemoryContext.SaveChangesAsync();
			return tache;
		}

		public async Task DeleteTaskById(int matricule)
		{
			var result = await dataBaseMemoryContext.Taches.FirstOrDefaultAsync(t => t.Matricule == matricule);
			if (result != null)
			{
				dataBaseMemoryContext.Taches.Remove(result);

				await dataBaseMemoryContext.SaveChangesAsync();
			}
		}

		public async Task<Tache> UpdateTask(Tache tache)
		{
			var tache1 = await dataBaseMemoryContext.Taches.FindAsync(tache.Matricule);
			dataBaseMemoryContext.Taches.Remove(tache1);
			Tache newtache = new()
			{
				Matricule = tache.Matricule,
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH= tache.EndDateH
			};
			await dataBaseMemoryContext.Taches.AddAsync(newtache);
			await dataBaseMemoryContext.SaveChangesAsync();
			return newtache;
		}
	}
}