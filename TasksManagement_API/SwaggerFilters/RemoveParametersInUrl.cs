using System.Net;
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
			HttpResponseMessage responseMessage = null;
			try
			{
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
				await Task.Delay(20);

				// Configurez HttpClientHandler pour ignorer la validation du certificat SSL
				var handler = new HttpClientHandler();
				handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

				// Envoyez votre requête POST à la nouvelle URL en utilisant HttpClient avec le HttpClientHandler configuré
				using var httpClient = new HttpClient(handler);
				responseMessage = await httpClient.PostAsync(newUrl, null);
				responseMessage.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				if (responseMessage == null)
				{
					responseMessage = new HttpResponseMessage();
				}
				responseMessage.StatusCode = HttpStatusCode.InternalServerError;
				responseMessage.ReasonPhrase = "RestHttpClient.SendRequest failed: {0}" + ex.Message.Trim();
			}
			return newUrl;
		}

	}

}

