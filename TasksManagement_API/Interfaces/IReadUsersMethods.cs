using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadUsersMethods
	{
		Task<TokenResult> GetToken(string email);
		bool CheckUserSecret(string secretPass);
		Task<ICollection<Utilisateur>> GetUsers(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null);
		Task<Utilisateur?> GetSingleUserByNameRole(string nom, Utilisateur.Privilege role);
		Task<string?> CheckExistedUser(Utilisateur Utilisateur);
		
	}
}