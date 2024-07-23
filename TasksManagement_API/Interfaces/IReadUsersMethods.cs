using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadUsersMethods
	{
		Task<TokenResult> GetToken(string email);                                         
		bool CheckUserSecret(string secretPass);
		Task<List<Utilisateur>> GetUsers();
		Task<Utilisateur> GetUserById(int id);
	}
}