using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IWriteUsersMethods
	{
		Task<string> GetToken(string email);
		bool CheckUserSecret(string secretPass);
		string EncryptUserSecret(string plainText);
		string DecryptUserSecret(string cipherText);
		Task<Utilisateur> CreateUser(Utilisateur utilisateur);
		Task<Utilisateur> SetUserPassword(string nom, string mdp);
		Task DeleteUserById(int id);
	}
}