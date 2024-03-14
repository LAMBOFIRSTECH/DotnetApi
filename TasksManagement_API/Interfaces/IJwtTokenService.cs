namespace TasksManagement_API.Interfaces
{
    public interface IJwtTokenService
	{
		string GetSigningKey();
		string GenerateJwtToken(string email);
	}
}