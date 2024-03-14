namespace Tasks_WEB_API.Interfaces
{
    public interface IReadUsersMethods
	{
		Task<List<Utilisateur>> GetUsers();
		Task<Utilisateur> GetUserById(int id);
	}
}