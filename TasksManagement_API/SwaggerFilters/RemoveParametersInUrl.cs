using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using TasksManagement_API.Interfaces;

namespace Tasks_WEB_API.SwaggerFilters
{
    public class RemoveParametersInUrl : IRemoveParametersIn
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IWriteUsersMethods writeUsersMethods;
		public RemoveParametersInUrl(IHttpContextAccessor httpContextAccessor, IWriteUsersMethods writeUsersMethods)
		{

			this.httpContextAccessor = httpContextAccessor;
			this.writeUsersMethods = writeUsersMethods;

		}

		public Task<bool> UsersManagement(List<string> queryParamsToRemove)
		{
			throw new NotImplementedException();
		}
		public async Task<string> AccessToken()
		{
			var newUrl = "";
			string requestUrl = httpContextAccessor.HttpContext!.Request.GetEncodedUrl();
			var uriBuilder = new UriBuilder(requestUrl);
			
			// Récupération et encryptage de la valeur secretUser
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			var secret = query["secretUser"];
			var newQuery = HttpUtility.ParseQueryString(uriBuilder.Query);
			newQuery.Set("secretUser", writeUsersMethods.EncryptUserSecret(secret!));

			// Reconstruction de la nouvelle Url
			var uriNoQuery = new UriBuilder(requestUrl) { Query = string.Empty }.Uri;
			newUrl = uriNoQuery.ToString() + newQuery.ToString();
			await Task.Delay(50);
			return uriNoQuery.ToString() ;
		}
	}
}