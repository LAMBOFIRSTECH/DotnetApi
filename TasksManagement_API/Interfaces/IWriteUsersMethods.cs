using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IWriteUsersMethods
	{
		Task<string> GetToken(string email);
		Task<Utilisateur> CreateUser(Utilisateur utilisateur);
		Task<Utilisateur> SetUserPassword(string nom, string mdp);
		Task DeleteUserById(int id);
	}
}