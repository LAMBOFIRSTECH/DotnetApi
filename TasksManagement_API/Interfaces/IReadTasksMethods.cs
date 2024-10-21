using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadTasksMethods
	{
		Task<ICollection<Tache>> GetTaches(Func<IQueryable<Tache>, IQueryable<Tache>>? filter = null);
	}
}