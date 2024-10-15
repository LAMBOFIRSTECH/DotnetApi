using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadTasksMethods
	{
		Task<List<Tache>> GetTaches(Func<IQueryable<Tache>, IQueryable<Tache>>? filter = null);
		Task<Tache> GetTaskByTitle(string? titre);
	}
}