using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadUsersMethods
	{
		Task<List<Utilisateur>> GetUsers();
		Task<Utilisateur> GetUserById(int id);
	}
}