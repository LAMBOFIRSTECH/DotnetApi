using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IWriteUsersMethods
	{
		Task<Utilisateur> CreateUser(Utilisateur utilisateur);
		Task<Utilisateur> UpdateUser(Utilisateur utilisateur);
		Task DeleteUserById(int id);
	}
}