using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IWriteUsersMethods
	{
		Task<Utilisateur> CreateUser(Utilisateur utilisateur);
		Task<Utilisateur> UpdateUser(Utilisateur utilisateur);
		Task<Utilisateur> setUserPassword(string nom, string mdp);
		Task<Utilisateur> PartialUpdateUser(int id,string nom,string mdp,string role,string email);
		Task DeleteUserById(int id);
	}
}