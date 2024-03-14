using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadTasksMethods
	{
		Task<List<Tache>> GetTaches();
		Task<Tache> GetTaskById(int? matricule);
	}
}