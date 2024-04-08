using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadUsersMethods
	{
		Task<string> GetToken(string email);                                          // change interface
		bool CheckUserSecret(string secretPass);
		Task<List<Utilisateur>> GetUsers();
		Task<Utilisateur> GetUserById(int id);
	}
}