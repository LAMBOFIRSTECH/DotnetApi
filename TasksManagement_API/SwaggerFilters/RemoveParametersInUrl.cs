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
		public async Task<bool> AccessToken(List<string> queryParamsToRemove)
		{
			HttpResponseMessage responseMessage = null;
			string requestUrl = httpContextAccessor.HttpContext!.Request.GetEncodedUrl();
			var uriBuilder = new UriBuilder(requestUrl);
			try
			{
				// Récupération et encryptage de la valeur secretUser
				var query = HttpUtility.ParseQueryString(uriBuilder.Query);
				var secret = query["secretUser"];
				writeUsersMethods.EncryptUserSecret(secret!);
				var newQuery = HttpUtility.ParseQueryString(uriBuilder.Query);
				newQuery.Set("secretUser", writeUsersMethods.EncryptUserSecret(secret!));

				// Reconstruction de la nouvelle Url
				var uriNoQuery = new UriBuilder(requestUrl) { Query = string.Empty }.Uri;
				var newUrl = uriNoQuery.ToString() + newQuery.ToString();
				await Task.Delay(20);

				// Envoyez votre requête POST à la nouvelle URL
				using var httpClient = new HttpClient();
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
				responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendRequest failed: {0}", ex);
			}
			return true;
		}
		
		
		
		public async Task<bool> EncryptParametersInUri(List<string> queryParamsToRemove)
		{
			string requestUrl = httpContextAccessor.HttpContext!.Request.GetEncodedUrl();
			var uriBuilder = new UriBuilder(requestUrl);
			try
			{
				// Supprimer les paramètres dont vous ne voulez pas dans votre URL
				var query = HttpUtility.ParseQueryString(uriBuilder.Query);
				var secret = query["secretUser"];
				writeUsersMethods.EncryptUserSecret(secret!);
				var newQuery = HttpUtility.ParseQueryString(uriBuilder.Query);
				newQuery.Set("secretUser", writeUsersMethods.EncryptUserSecret(secret!));
				var toto = uriBuilder.Uri.Segments;


				string url = uriBuilder.Uri.ToString() + newQuery;


				// Utilisez la nouvelle URL construite sans les paramètres
				var newUrl = uriBuilder.Uri.ToString();
				Console.WriteLine("----------------------------------");

				// Envoyez votre requête POST à la nouvelle URL
				using var httpClient = new HttpClient();
				var response = await httpClient.PostAsync(newUrl, null);
				Console.WriteLine(response.RequestMessage!.RequestUri);
				response.EnsureSuccessStatusCode();

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Une erreur est survenue lors de la suppression des paramètres de l'URL {uriBuilder} : {ex.Message}");
				throw; // Renvoyer l'exception pour la gérer à un niveau supérieur si nécessaire
			}
		}


	}
}
