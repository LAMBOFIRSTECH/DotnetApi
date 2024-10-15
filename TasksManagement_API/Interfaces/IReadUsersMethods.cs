using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadUsersMethods
	{
		Task<TokenResult> GetToken(string email);
		bool CheckUserSecret(string secretPass);
		Task<List<Utilisateur>> GetUsers(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null);
		Task<Utilisateur?> GetSingleUserByNameRole(string nom, Utilisateur.Privilege role);
		
	}
}